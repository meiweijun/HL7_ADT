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
            string str = "";
            str = Generate();
            Analyzing(str);
        }

        static void Analyzing(string hl7Message)
        {
            try
            {
                // 创建 PipeParser 实例
                var parser = new PipeParser();

                // 解析 HL7 消息字符串为 ADT_A01 消息对象
                IMessage message = parser.Parse(hl7Message);

                // 如果消息是 ADT_A01 类型，则转换并处理
                if (message is ADT_A01 adtA01)
                {
                    // 处理 MSH 段
                    ProcessMSH(adtA01.MSH);

                    // 处理 EVN 段
                    ProcessEVN(adtA01.EVN);

                    // 处理 PID 段
                    ProcessPID(adtA01.PID);

                    // 处理 PV1 段
                    ProcessPV1(adtA01.PV1);
                }
                else
                {
                    Console.WriteLine("The parsed message is not an ADT_A01 type.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while parsing the HL7 message: {ex.Message}");
            }
        }

        static void ProcessMSH(MSH msh)
        {
            Console.WriteLine("Processing MSH segment:");
            Console.WriteLine($"Sending Application: {msh.SendingApplication.NamespaceID.Value}");
            Console.WriteLine($"Sending Facility: {msh.SendingFacility.NamespaceID.Value}");
            Console.WriteLine($"Receiving Application: {msh.ReceivingApplication.NamespaceID.Value}");
            Console.WriteLine($"Receiving Facility: {msh.ReceivingFacility.NamespaceID.Value}");
            Console.WriteLine($"Message Type: {msh.MessageType.MessageCode.Value}^{msh.MessageType.TriggerEvent.Value}");
            Console.WriteLine($"Message Control ID: {msh.MessageControlID.Value}");
            Console.WriteLine($"Version ID: {msh.VersionID.VersionID.Value}");
        }

        static void ProcessEVN(EVN evn)
        {
            Console.WriteLine("Processing EVN segment:");
            Console.WriteLine($"Event Type Code: {evn.EventTypeCode.Value}");
            Console.WriteLine($"Recorded Date/Time: {evn.RecordedDateTime.Time.Value}");
        }

        static void ProcessPID(PID pid)
        {
            Console.WriteLine("Processing PID segment:");
            Console.WriteLine($"Patient ID: {pid.GetPatientIdentifierList(0).IDNumber.Value}");
            Console.WriteLine($"Patient Name: {pid.GetPatientName(0).FamilyName.Surname.Value}, {pid.GetPatientName(0).GivenName.Value}");
            Console.WriteLine($"Date of Birth: {pid.DateTimeOfBirth.Time.Value}");
            Console.WriteLine($"Sex: {pid.AdministrativeSex.Value}");
            Console.WriteLine($"Race: {pid.GetRace(0).Identifier.Value}");
            Console.WriteLine($"Address: {pid.GetPatientAddress(0).StreetAddress.StreetOrMailingAddress.Value}, {pid.GetPatientAddress(0).City.Value}, {pid.GetPatientAddress(0).StateOrProvince.Value} {pid.GetPatientAddress(0).ZipOrPostalCode.Value}");
        }

        static void ProcessPV1(PV1 pv1)
        {
            Console.WriteLine("Processing PV1 segment:");
            Console.WriteLine($"Set ID Patient Visit: {pv1.SetIDPV1}.Value"); // 这里假设可以直接获取值
            Console.WriteLine($"Patient Class: {pv1.PatientClass.Value}");
            Console.WriteLine($"Assigned Patient Location: {pv1.AssignedPatientLocation.PointOfCare.Value}");
            Console.WriteLine($"Attending Doctor: {pv1.GetAttendingDoctor(0).IDNumber.Value}");
        }
        static string Generate()
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

                return hl7Message;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return null;
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