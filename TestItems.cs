using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Data;


using sys;
using PinsDataLib;
using CalibrationTestItem;
using Current;
using Ins.IO;
using Ins.Dps;
using Ins.VNA;
using Ins.IQ;
using Ins.IQxel;
using App.Template;
using Site.Hardware;
using SiteTimer;
using System.IO;

namespace CustomerProgram
{
    public class TestItems
    {
        /// <summary>
        /// 批次测试开始时调用。每批次测试只调用一次
        /// Call "TEST_START" before lot test, executing once for each lot.
        /// </summary>
        public TEST TEST_START()
        {
            //TODO: 添加需要执行的代码，例如板卡初始化等等。
            //TODO: Adding Test Code need to be executed, such as initializing instrument cards;
            //Load pattern file from Test Plan
            string[] patternFileList = Customerdata.GlobalTables["Global"].AsEnumerable().Select(c => c.Field<string>("PatternFile")).ToArray();
            //TemplateDigital.LoadPatterns(patternFileList);
            foreach (string patternFile in patternFileList)
            {
                if (patternFile != null)
                {
                    string pattern = Path.Combine(Customerdata.TestProgramDirectory, "Patterns", patternFile);

                    if (File.Exists(pattern))
                    {
                        Digital.Patterns(pattern).Load();
                    }
                    else
                    {
                        throw new Exception($"Wrong : Pattern file {patternFile} didn't exist.");
                    }
                }
            }

            //函数执行结束，无异常发生。
            //Function execution is done, no exception occurs;
            return TEST.PASS;
        }

        /// <summary>
        /// 批次测试结束时调用。每批次测试只调用一次。
        /// Call "TEST_END" after lot test, executing once for each lot.
        /// </summary>
        public TEST TEST_END()
        {
            //TODO:添加需要执行的代码，例如板卡取消初始化等等。


            //函数执行结束，无异常发生。
            return TEST.PASS;
        }

        /// <summary>
        /// 每次Flow测试之前调用。
        /// Call "FLOW_START" before flow execution, once for each flow.
        /// </summary>
        public TEST FLOW_START()
        {
            //TODO:添加需要执行的代码，例如测试前板卡状态设置等等。
            //TODO: Adding Test Code need to be executed, such as setup instrument status;


            //函数执行结束，无异常发生。
            //Function execution is done, no exception occurs;
            return TEST.PASS;
        }

        /// <summary>
        /// 每次Flow测试之后调用。
        /// Call "FLOW_END" after flow execution, once for each flow.
        /// </summary>
        public TEST FLOW_END()
        {
            //TODO:添加需要执行的代码，例如测试后板卡状态复位等等。
            //TODO: Adding Test Code need to be executed, such as reset instrument cards;


            //函数执行结束，无异常发生。
            //Function execution is done, no exception occurs;
            return TEST.PASS;
        }



        /*
         *
         * 
         * CPCode
         * 
         * 

         * 
         * 
         */
        //// <summary>
        //// FLOW_XXX Function
        //// 
        //// 
        //// </summary>
        //public TEST FLOW_XXX()
        //{
        //    try
        //    {
        //        //TODO:添加需要执行的代码，例如测试后板卡状态复位等等。
        //        //    TODO: Adding Test Code need to be executed, such as reset instrument cards;
        //        Result.TestLimit(osResult);
        //        if (SitesInfo.ActiveSites.Count == 0)
        //            return TEST.PASS;
        //        return TEST.PASS;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"{Customerdata.TestItemName } occured exception,exception message is {ex.Message}");
        //        return TEST.FAIL;
        //    }
        //}


/// <summary>
/// FLOW_OS
/// VCC= GND,Force -100uA to IOPINS,Measure Voltage
/// </summary>
        public TEST FLOW_OS()
        {
            try
            {
                TemplateDps.ApplyVoltage("VCC", 0, 0.5);
                PinsData osResult = new PinsData();
                TemplatePpmu.OS(Customerdata.Pins, Customerdata.Force, ref osResult);
                TemplateDps.Reset("VCC");

                Result.TestLimit(osResult);
                if (SitesInfo.ActiveSites.Count == 0)
                    return TEST.PASS;
                return TEST.PASS;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Customerdata.TestItemName } occured exception,exception message is {ex.Message}");
                return TEST.FAIL;
            }
        }

        /// <summary>
        /// FLOW_PowerShort_1
        /// VCC= 5.25V,IOPINS=0v
        /// Measure VCC Current,Set forceIRange=0.5,measureIRange=2.5e-3 in TemplateDps.PowerShort()
        ///Reslut limit from -1mA to 10mA
        /// </summary>
        public TEST FLOW_PowerShort_1()
        {
            try
            {
                TemplatePpmu.ApplyVoltage("IOPINS", 0, 0.5);
                PinsData powerShortResult = new PinsData();
                TemplateDps.PowerShort(Customerdata.Pins, Customerdata.Force, 0.5, 2.5e-3, ref powerShortResult);
                TemplatePpmu.Reset("IOPINS");

                Result.TestLimit(powerShortResult);
                if (SitesInfo.ActiveSites.Count == 0)
                    return TEST.PASS;
                return TEST.PASS;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Customerdata.TestItemName } occured exception,exception message is {ex.Message}");
                return TEST.FAIL;
            }
        }
        /// <summary>
        /// FLOW_PowerShort_2
        /// VCC= 0.1V,IOPINS=0v
        /// Measure VCC Current,Set forceIRange=25e-3,measureIRange=2.5e-6 in TemplateDps.PowerShort()
        /// Reslut limit from -1uA to 10uA
        /// </summary>
        public TEST FLOW_PowerShort_2()
        {
            try
            {
                TemplatePpmu.ApplyVoltage("IOPINS", 0, 0.5);
                PinsData powerShortResult = new PinsData();
                TemplateDps.PowerShort(Customerdata.Pins, Customerdata.Force, 25e-3, 2.5e-6, ref powerShortResult);
                TemplatePpmu.Reset("IOPINS");

                Result.TestLimit(powerShortResult);
                if (SitesInfo.ActiveSites.Count == 0)
                    return TEST.PASS;
                return TEST.PASS;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Customerdata.TestItemName } occured exception,exception message is {ex.Message}");
                return TEST.FAIL;
            }
        }

        /// <summary>
        /// FLOW_Static_ICC
        /// VCC= 5.25V,OE = H
        /// Measure VCC Current
        /// </summary>
        public TEST FLOW_Static_ICC()
        {
            try
            {
                TemplateDps.ApplyVoltage(Customerdata.Pins, Customerdata.Force, 0.5);
                TemplatePpmu.ApplyVoltage("OE", 3, 0.5);
                Hardware.Wait(2);


                PinsData staticICC = new PinsData();
                Dps.Pins(Customerdata.Pins).Result(ref staticICC);
                Result.TestLimit(staticICC);

                TemplatePpmu.Reset("OE");
                TemplateDps.Reset(Customerdata.Pins);
                if (SitesInfo.ActiveSites.Count == 0)
                    return TEST.PASS;
                return TEST.PASS;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Customerdata.TestItemName } occured exception,exception message is {ex.Message}");
                return TEST.FAIL;
            }
        }

        /// <summary>
        /// FLOW_Dynamic_ICC
        /// VCC= 5.25V,Run pattern:ti245_func
        /// Measure VCC Current
        /// </summary>
        public TEST FLOW_Dynamic_ICC()
        {
            try
            {
                TemplateDps.ApplyVoltage(Customerdata.Pins, Customerdata.Force, 0.5);  //DPS channel apply voltage

                Digital.SetTimingLevels(false, true, true);  //bool connectAllPins, bool loadLevels = true, bool loadTiming = true
                Digital.Pins("IOPINS").Connect();
                Hardware.Wait(2);

                string patternFunc = Customerdata.Arg[0].ParamValue;
                RunPattern(patternFunc, out SiteDouble patResultFunc, true, 10);
                Result.TestLimit(patResultFunc);
                if (SitesInfo.ActiveSites.Count == 0) return TEST.PASS;
                Digital.Pins("IOPINS").Disconnect();

                PinsData dynamicICC = new PinsData();
                Dps.Pins(Customerdata.Pins).Result(ref dynamicICC);
                Result.TestLimit(dynamicICC);
                if (SitesInfo.ActiveSites.Count == 0) return TEST.PASS;


                TemplatePpmu.Reset("IOPINS");
                TemplateDps.Reset(Customerdata.Pins);

                return TEST.PASS;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Customerdata.TestItemName}  occurred exception, exception message is {ex.Message}");
                return TEST.FAIL;
            }
        }

        // <summary>
        // FLOW_VOH Function
        // 
        // 
        // </summary>
        public TEST FLOW_VOH()
        {
            try
            {
                //TODO:添加需要执行的代码，例如测试后板卡状态复位等等。
                //    TODO: Adding Test Code need to be executed, such as reset instrument cards;
                TemplateDps.ApplyVoltage("VCC", 4.75, 0.5);
                TemplatePpmu.ApplyVoltage("CTRLS", 0, 0.5);
                TemplatePpmu.ApplyVoltage("PORTB", 3, 0.5);

                Ppmu.Pins(Customerdata.Pins).Disconnect();
                Ppmu.Pins(Customerdata.Pins).Mode.FIMV();
                Ppmu.Pins(Customerdata.Pins).CurrentRange._5mA();
                Ppmu.Pins(Customerdata.Pins).VoltageClamp(5, -2);
                Ppmu.Pins(Customerdata.Pins).SetCurrent(Customerdata.Force);
                Ppmu.Pins(Customerdata.Pins).Connect();
                Ppmu.Pins(Customerdata.Pins).Gate.On();
                Hardware.Wait(2);

                PinsData voh = new PinsData();
                Ppmu.Pins(Customerdata.Pins).Result(ref voh);
                Result.TestLimit(voh);
                if (SitesInfo.ActiveSites.Count == 0)
                    return TEST.PASS;
                TemplateDps.Reset("VCC");
                TemplatePpmu.Reset("IOPINS");

                return TEST.PASS;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Customerdata.TestItemName } occured exception,exception message is {ex.Message}");
                return TEST.FAIL;
            }
        }


        // <summary>
        // FLOW_VOL Function
        // 
        // 
        // </summary>
        public TEST FLOW_VOL()
        {
            try
            {
                //TODO:添加需要执行的代码，例如测试后板卡状态复位等等。
                //    TODO: Adding Test Code need to be executed, such as reset instrument cards;
                TemplateDps.ApplyVoltage("VCC",4.75,0.5);
                TemplatePpmu.ApplyVoltage("CTRLS",0,0.5);
                TemplatePpmu.ApplyVoltage("PORTB",0,0.5);
                Hardware.Wait(2);
                Ppmu.Pins(Customerdata.Pins).Disconnect();
                Ppmu.Pins(Customerdata.Pins).Mode.FIMV();
                Ppmu.Pins(Customerdata.Pins).CurrentRange._50mA();
                Ppmu.Pins(Customerdata.Pins).VoltageClamp(5,-2);
                Ppmu.Pins(Customerdata.Pins).SetCurrent(Customerdata.Force);
                Ppmu.Pins(Customerdata.Pins).Connect();
                Ppmu.Pins(Customerdata.Pins).Gate.On();
                Hardware.Wait(3);

                PinsData vol = new PinsData();
                Ppmu.Pins(Customerdata.Pins).Result(ref vol);
                Result.TestLimit(vol);
                if (SitesInfo.ActiveSites.Count == 0)
                    return TEST.PASS;
                TemplateDps.Reset("VCC");
                TemplatePpmu.Reset("IOPINS");

                return TEST.PASS;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Customerdata.TestItemName } occured exception,exception message is {ex.Message}");
                return TEST.FAIL;
            }
        }



        private void RunPattern(string patternName, out SiteDouble patResult, bool printFailVactors = false, long printMaxLine = 0)
        {
            patResult = new SiteDouble();

            try
            {
                Digital.Patterns(patternName).Run();
                Digital.Patterns(patternName).HaltWait();
                Digital.Patterns(patternName).Result(ref patResult, printFailVactors, printMaxLine);//4k 4096

            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception occurred in RunPattern function:\r\n" + ex.Message);

                throw ex;
            }
        }
        // <summary>
        // FLOW_Func_nom Function
        // 
        // 
        // </summary>
        public TEST FLOW_Func_nom()
        {
            try
            {
                //TODO:添加需要执行的代码，例如测试后板卡状态复位等等。
                //    TODO: Adding Test Code need to be executed, such as reset instrument cards;
                TemplateDps.ApplyVoltage("VCC",Customerdata.Force,0.5);
                Digital.SetTimingLevels(false, true, true);
                Digital.Pins(Customerdata.Pins).Connect();
                Hardware.Wait(2);
                string patternFunc = Customerdata.Arg[0].ParamValue;
                RunPattern(patternFunc, out SiteDouble patResultFunc, true, 10);

                Result.TestLimit(patResultFunc);
                if (SitesInfo.ActiveSites.Count == 0)
                    return TEST.PASS;
                Digital.Pins(Customerdata.Pins).Disconnect();
                TemplateDps.Reset("VCC");
                return TEST.PASS;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Customerdata.TestItemName } occured exception,exception message is {ex.Message}");
                return TEST.FAIL;
            }
        }

        /// <summary>
        /// VCC = 5V, Run pattern: ti245_time
        /// </summary>
        /// <returns></returns>
        public TEST FLOW_Func_cap()
        {
            try
            {
                string captureData = "captureData";
                string patternCapture = Customerdata.Arg[0].ParamValue.Trim();

                //---1）Check bin文件中“B8”资源
                //---2）定义在DDR中开辟 Capture的存放地址“captureData”
                Digital.Capture(patternCapture, "B8").SampleSize(captureData, 8);

                TemplateDps.ApplyVoltage("VCC", Customerdata.Force, 0.5);
                Digital.SetTimingLevels(false, true, true);
                Digital.Pins(Customerdata.Pins).Connect();
                Hardware.Wait(2);

                RunPattern(patternCapture, out SiteDouble resultCapture, true, 10);
                Result.TestLimit(resultCapture);
                if (SitesInfo.ActiveSites.Count == 0) return TEST.PASS;

                Digital.Pins(Customerdata.Pins).Disconnect();
                TemplateDps.Reset("VCC");

                SiteDouble sdSignalWave = new SiteDouble();
                Digital.Capture(patternCapture, "B8").Result(captureData, ref sdSignalWave);

                foreach (site currSite in SitesInfo.SitesLoop)
                {
                    List<string> decList = new List<string>();
                    for (int i = 0; i < sdSignalWave.SiteArrayValue[currSite].Length; i++)
                    {
                        //将Capture到的Double类型结果强转为int类型
                        //再将int类型转换为 十六进制 类型
                        decList.Add($"{((Int32)sdSignalWave.SiteArrayValue[currSite][i]).ToString("X")}");
                    }

                    Result.Message($"The result of captured data on {sdSignalWave.SiteName[currSite]} is : {string.Join(" ", decList)}");
                    decList.Clear();
                }

                return TEST.PASS;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Customerdata.TestItemName}  occurred exception, exception message is {ex.Message}");
                Result.Message($"FLOW_FUN_UNID occurred exception, exception message is {ex.Message}");
                return TEST.FAIL;
            }
        }

    }
}