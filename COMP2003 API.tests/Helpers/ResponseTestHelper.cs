﻿using System;
using System.Collections.Generic;
using System.Text;
using COMP2003_API.Responses;

namespace COMP2003_API.Tests.Helpers
{
    public class ResponseTestHelper
    {
        public static DeletionResult GetSuccessfulAcccountDeletionResult()
        {
            DeletionResult result = new DeletionResult
            {
                Success = true,
                Message = "This account has been deleted."
            };
            return result;
        }

        public static DeletionResult GetFailedAccountDeletionResult()
        {
            DeletionResult result = new DeletionResult
            {
                Success = false,
                Message = "Deletion failed - account does not exist."
            };
            return result;
        }

        public static DeletionResult GetSuccessfulBookingDeletionResult()
        {
            DeletionResult result = new DeletionResult
            {
                Success = true,
                Message = "This booking has been deleted."
            };
            return result;
        }

        public static DeletionResult GetFailedBookingDeletionResult()
        {
            DeletionResult result = new DeletionResult
            {
                Success = false,
                Message = "Deletion failed - booking does not exist."
            };
            return result;
        }
    }
}
