using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemAlert.Data.Model.Common
{
    public class ResponseCode : Common.Model.ResponseCode
    {
        public static string GetCode(CodeNumbers code)
        {
            var codeValue = new ResponseCode(code).Code;
            if (string.IsNullOrEmpty(codeValue))
                return Enum.GetName(typeof(CodeNumbers), code);
            return codeValue;
        }

        public ResponseCode(CodeNumbers code) : base((int)code) { }


        protected override Dictionary<int, ResponseInfo> ResponseItems() => new Dictionary<int, ResponseInfo>()
        {
            //ALT-S-XXX ALERT SUCCESS
            {(int)CodeNumbers.ALTS001, new ResponseInfo(){ Code = "ALT-S-001", Message = "Alert fetched successfully"}},
            {(int)CodeNumbers.ALTS002, new ResponseInfo(){ Code = "ALT-S-002", Message = "Alert created successfully"}},
            {(int)CodeNumbers.ALTS003, new ResponseInfo(){ Code = "ALT-S-003", Message = "Alert updated successfully"}},
            {(int)CodeNumbers.ALTS004, new ResponseInfo(){ Code = "ALT-S-004", Message = "Alert deleted successfully"}},
            {(int)CodeNumbers.ALTS005, new ResponseInfo(){ Code = "ALT-S-005", Message = "Alert configuration fetched successfully"}},
            {(int)CodeNumbers.ALTS006, new ResponseInfo(){ Code = "ALT-S-006", Message = "Alert configuration created successfully"}},
            {(int)CodeNumbers.ALTS007, new ResponseInfo(){ Code = "ALT-S-007", Message = "Alert configuration updated successfully"}},
            {(int)CodeNumbers.ALTS008, new ResponseInfo(){ Code = "ALT-S-008", Message = "Alert configuration deleted successfully"}},
            {(int)CodeNumbers.ALTS009, new ResponseInfo(){ Code = "ALT-S-009", Message = "Alert configuration enabled successfully"}},
            {(int)CodeNumbers.ALTS010, new ResponseInfo(){ Code = "ALT-S-010", Message = "Alert configuration disabled successfully"}},
            {(int)CodeNumbers.ALTS011, new ResponseInfo(){ Code = "ALT-S-011", Message = "Alert type configuration fetched successfully"}},
            {(int)CodeNumbers.ALTS012, new ResponseInfo(){ Code = "ALT-S-012", Message = "Alert type configuration created successfully"}},
            {(int)CodeNumbers.ALTS013, new ResponseInfo(){ Code = "ALT-S-013", Message = "Alert type configuration updated successfully"}},
            {(int)CodeNumbers.ALTS014, new ResponseInfo(){ Code = "ALT-S-014", Message = "Alert type configuration deleted successfully"}},
            {(int)CodeNumbers.ALTS015, new ResponseInfo(){ Code = "ALT-S-015", Message = "Alert mesaage was processed and email was sent successfully"}},
            {(int)CodeNumbers.ALTS016, new ResponseInfo(){ Code = "ALT-S-016", Message = "Alert mesaage was processed and but no email was sent"}},
            {(int)CodeNumbers.ALTS017, new ResponseInfo(){ Code = "ALT-S-017", Message = "Email sent successfully"}},
            //ALT-E-XXX ALERT ERROR
            {(int)CodeNumbers.ALTE001, new ResponseInfo(){ Code = "ALT-E-001", Message = "Alert could not be fetched"}},
            {(int)CodeNumbers.ALTE002, new ResponseInfo(){ Code = "ALT-E-002", Message = "Alert could not be created"}},
            {(int)CodeNumbers.ALTE003, new ResponseInfo(){ Code = "ALT-E-003", Message = "Alert could not be updated"}},
            {(int)CodeNumbers.ALTE004, new ResponseInfo(){ Code = "ALT-E-004", Message = "Alert could not be deleted"}},
            {(int)CodeNumbers.ALTE005, new ResponseInfo(){ Code = "ALT-E-005", Message = "Alert configuration could not be fetched"}},
            {(int)CodeNumbers.ALTE006, new ResponseInfo(){ Code = "ALT-E-006", Message = "Alert configuration could not be created"}},
            {(int)CodeNumbers.ALTE007, new ResponseInfo(){ Code = "ALT-E-007", Message = "Alert configuration could not be updated"}},
            {(int)CodeNumbers.ALTE008, new ResponseInfo(){ Code = "ALT-E-008", Message = "Alert configuration could not be deleted"}},
            {(int)CodeNumbers.ALTE009, new ResponseInfo(){ Code = "ALT-E-009", Message = "Alert configuration could not be enabled"}},
            {(int)CodeNumbers.ALTE010, new ResponseInfo(){ Code = "ALT-E-010", Message = "Alert configuration could not be disabled"}},
            {(int)CodeNumbers.ALTE011, new ResponseInfo(){ Code = "ALT-E-011", Message = "Alert type configuration could not be fetched"}},
            {(int)CodeNumbers.ALTE012, new ResponseInfo(){ Code = "ALT-E-012", Message = "Alert type configuration could not be created"}},
            {(int)CodeNumbers.ALTE013, new ResponseInfo(){ Code = "ALT-E-013", Message = "Alert type configuration could not be updated"}},
            {(int)CodeNumbers.ALTE014, new ResponseInfo(){ Code = "ALT-E-014", Message = "Alert type configuration could not be deleted"}},
            {(int)CodeNumbers.ALTE015, new ResponseInfo(){ Code = "ALT-E-015", Message = "Email could not be sent"}},
            {(int)CodeNumbers.ALTE016, new ResponseInfo(){ Code = "ALT-E-016", Message = "CoolDown value in alert configuration could not be fetched"}},
            {(int)CodeNumbers.ALTE017, new ResponseInfo(){ Code = "ALT-E-017", Message = "Email template could not be found"}},
            {(int)CodeNumbers.ALTE018, new ResponseInfo(){ Code = "ALT-E-018", Message = "Alert customer could not be found"}},
        };


        public enum CodeNumbers : int
        {
            //ALT-S-XXX ALERT SUCCESS
            ALTS001,
            ALTS002,
            ALTS003,
            ALTS004,
            ALTS005,
            ALTS006,
            ALTS007,
            ALTS008,
            ALTS009,
            ALTS010,
            ALTS011,
            ALTS012,
            ALTS013,
            ALTS014,
            ALTS015,
            ALTS016,
            ALTS017,
            ALTS018,


            //ALT-E-XXX ALERT ERROR
            ALTE001,
            ALTE002,
            ALTE003,
            ALTE004,
            ALTE005,
            ALTE006,
            ALTE007,
            ALTE008,
            ALTE009,
            ALTE010,
            ALTE011,
            ALTE012,
            ALTE013,
            ALTE014,
            ALTE015,
            ALTE016,
            ALTE017,
            ALTE018,
        };

    }
}
