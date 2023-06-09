﻿using GraduationProject.Models;
using GraduationProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GraduationProject.Controllers
{
    //[Authorize(Roles = "SuperAdmin,Admin")]
    public class MangeTestCasesController : ControllerBase
    {
        private readonly ProjectDbContext db;
        public MangeTestCasesController(ProjectDbContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// inesrt testcases for problems
        /// </summary>
        /// <param name="p_id">problem ID</param>
        /// <param name="TestCases">TestCases as base 64</param>
        /// <returns>return 422 and list of errors if data not valid , 201 if data is created , else 500</returns>
        [HttpPost("problem/{p_id:int}/testcases")]
        public ActionResult PostTestCases([FromRoute] int? p_id, [FromForm] TestCasesBinding TestCase)
        {

            if (p_id is null )
                ModelState.AddModelError("Problem id", "Non valid data");
            if(TestCase is null 
                || TestCase.InputCase is null || TestCase.InputCase.Length == 0
                || TestCase.OutputCase is null || TestCase.OutputCase.Length == 0)

                ModelState.AddModelError("Testcases data", "Non valid data");

            if (!ModelState.IsValid)
            {
                var Errors = ModelState.Values.SelectMany(v => v.Errors);

                return StatusCode(422, Errors);
            }

            bool IsCreated = TestCasesServices.IsCreated(ref TestCase, p_id);
            if (IsCreated)
            {
                return StatusCode((int)HttpStatusCode.Created);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
        /// <summary>
        /// return all testcases input and output as string with there id 
        /// </summary>
        /// <param name="p_id">problem ID</param>
        /// <returns>return 200 and all cases of found , 204 if case not found</returns>
        [HttpGet("problem/{p_id:int}/testcases")]
        public ActionResult GetTestCases([FromRoute] int? p_id)
        {
            if (p_id is null)
                return StatusCode(422);

            var TestCases =
                from Input_Cases in db.InputCases
                join Output_Cases in db.OutputCases on
                Input_Cases.Id equals Output_Cases.InputId
                select new
                {
                    InputID = Input_Cases.Id,
                    FileInput = Input_Cases.Input,
                    OutputID = Output_Cases.Id,
                    FileOutput = Output_Cases.Output
                };

            if (TestCases is null)
                return StatusCode((int)HttpStatusCode.NoContent);
            return Ok(TestCases);
        }
        /// <summary>
        /// Delete test case and it's related one 
        /// </summary>
        /// <param name="TestID">Test Case ID</param>
        /// <returns>422
        /// if not found , 
        /// 204 if found ,
        /// 500 if unknow errors</returns>
        [HttpDelete("testcase")]
        public ActionResult DeleteTestCase(string? testid)
        {
            int IsOperated = TestCasesServices.DeleteTestCase(testid??"");
            if (IsOperated == 0)
                return StatusCode(422, "Not Found");
            else if (IsOperated == 1)
                return StatusCode((int)HttpStatusCode.NoContent);
            return StatusCode((int)HttpStatusCode.InternalServerError);

        }
    }
}
