using System;
using NHapi.Base.Model;
using NHapi.Base.Parser;
using NHapi.Model.V251.Datatype;
using NHapi.Model.V251.Message;
using NHapi.Model.V251.Segment;
namespace A01

{
    class Program
    {
        static void Main()
        {
            try
            {
                // 创建ADT_A01消息实例
                var adtA01 = new ADT_A01();

                // 设置消息头(MSH段)
                SetupMSH(adtA01.MSH);

                // 设置事件信息(EVN段)
                SetupEVN(adtA01.EVN);

                // 设置患者信息(PID段)
                SetupPID(adtA01.PID);

                // 设置就诊信息(PV1段)
                SetupPV1(adtA01.PV1);

                // 使用管道编码器生成HL7消息
                var parser = new PipeParser();
                string hl7Message = parser.Encode(adtA01);

                Console.WriteLine("Generated HL7 ADT^A01 Message:");
                Console.WriteLine(hl7Message.Replace("\r", "\n")); // 增强可读性
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        static void SetupMSH(MSH msh)
        {
            // MSH-1 字段分隔符 (默认|)
            msh.FieldSeparator.Value = "|";

            // MSH-2 编码字符
            msh.EncodingCharacters.Value = "^~\\&";

            // MSH-3 发送系统
            msh.SendingApplication.NamespaceID.Value = "HIS_SYS";

            // MSH-4 发送机构
            msh.SendingFacility.NamespaceID.Value = "HOSPITAL_A";

            // MSH-5 接收系统
            msh.ReceivingApplication.NamespaceID.Value = "EMR_SYS";

            // MSH-6 接收机构
            msh.ReceivingFacility.NamespaceID.Value = "CLINIC_B";

            // MSH-7 消息时间戳
            msh.DateTimeOfMessage.Time.SetLongDate(DateTime.Now);

            // MSH-9 消息类型
            msh.MessageType.MessageCode.Value = "ADT";
            msh.MessageType.TriggerEvent.Value = "A01";
            msh.MessageType.MessageStructure.Value = "ADT_A01";

            // MSH-10 消息控制ID
            msh.MessageControlID.Value = Guid.NewGuid().ToString().Substring(0, 20);

            // MSH-11 处理模式
            msh.ProcessingID.ProcessingID.Value = "P"; // Production

            // MSH-12 版本号
            msh.VersionID.VersionID.Value = "2.5.1";
        }

        static void SetupEVN(EVN evn)
        {
            evn.EventTypeCode.Value = "A01";
            evn.RecordedDateTime.Time.SetLongDate(DateTime.Now);
        }

        static void SetupPID(PID pid)
        {
            // PID-3 患者ID
            var cx = pid.GetPatientIdentifierList(0);
            cx.IDNumber.Value = "PAT-123456";
            cx.AssigningAuthority.NamespaceID.Value = "HOSPITAL_A";

            // PID-5 患者姓名
            var xpn = pid.GetPatientName(0);
            xpn.FamilyName.Surname.Value = "Smith";
            xpn.GivenName.Value = "John";
            xpn.NameTypeCode.Value = "L"; // Legal

            // PID-7 出生日期
            //pid.DateTimeOfBirth.Time.SetDate(1980, 1, 15);
            pid.DateTimeOfBirth.Time.setDatePrecision(1980,1,15);

            // PID-8 性别
            pid.AdministrativeSex.Value = "M"; // Male

            // PID-10 种族（使用CDC编码）
            var ce = pid.GetRace(0);
            ce.Identifier.Value = "2106-3"; // White
            ce.Text.Value = "White";
            ce.NameOfCodingSystem.Value = "CDCREC";

            // PID-11 地址
            var xad = pid.GetPatientAddress(0);
            xad.StreetAddress.StreetOrMailingAddress.Value = "123 MAIN ST";
            xad.City.Value = "ANYTOWN";
            xad.StateOrProvince.Value = "CA";
            xad.ZipOrPostalCode.Value = "90210";
            xad.Country.Value = "USA";
        }

        static void SetupPV1(PV1 pv1)
        {
            // PV1-1 序号
            // pv1.SetIDPatientVisit.Value = "1";
            //pv1.GetField(0)
            //pv1.GetSetIDPatientVisit(0).Value = "1";
            //pv1.SetIDPV1
            // 设置访问ID（SetIDPatientVisit）
            pv1.SetIDPV1.Value = "1";
            // PV1-2 患者类型
            pv1.PatientClass.Value = "I"; // Inpatient

            // PV1-3 就诊地点
            var pl = pv1.AssignedPatientLocation;
            pl.Facility.NamespaceID.Value = "HOSPITAL_A";
            pl.PointOfCare.Value = "ICU";
            pl.Room.Value = "BED-01";

            // PV1-7 主治医生
            var xcn = pv1.GetAttendingDoctor(0);
            xcn.IDNumber.Value = "DOC-987";
            xcn.FamilyName.Surname.Value = "Williams";
            xcn.GivenName.Value = "Sarah";
            xcn.IdentifierTypeCode.Value = "MD";

            // PV1-44 入院类型
            pv1.AdmissionType.Value = "EMER"; // Emergency

            // PV1-52 VIP标识
            pv1.VIPIndicator.Value = "N"; // Not a VIP
        }
    }
}