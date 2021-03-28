using System;
using System.Collections.Generic;
using System.Text;
using COMP2003_API.Responses;

namespace COMP2003_API.Tests.Helpers
{
    public class ResponseTestHelper
    {
        public static DeletionResult GetSuccessfulDeletionResult()
        {
            DeletionResult result = new DeletionResult
            {
                Success = true,
                Message = "This account has been deleted."
            };
            return result;
        }

        public static DeletionResult GetFailedDeletionResult()
        {
            DeletionResult result = new DeletionResult
            {
                Success = false,
                Message = "Deletion failed - account does not exist."
            };
            return result;
        }
    }
}
