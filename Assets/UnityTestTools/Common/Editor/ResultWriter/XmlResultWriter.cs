using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using UnityEngine;

namespace UnityTest
{
    public class XmlResultWriter
    {
        private readonly StringBuilder m_ResultWriter = new StringBuilder();
        private int m_Indend;
        private readonly string m_SuiteName;
        private readonly ITestResult[] m_Results;

        public XmlResultWriter(string suiteName, ITestResult[] results)
        {
            m_SuiteName = suiteName;
            m_Results = results;
        }

        private const string k_NUnitVersion = "2.6.2-Unity";

        public string GetTestResult()
        {
            InitializeXmlFile(m_SuiteName, new ResultSummarizer(m_Results));
            foreach (var result in m_Results)
            {
                WriteResultElement(result);
            }
            TerminateXmlFile();
            return m_ResultWriter.ToString();
        }

        private void InitializeXmlFile(string resultsName, ResultSummarizer summaryResults)
        {
            WriteHeader();

            DateTime now = DateTime.Now;
            var attributes = new Dictionary<string, string>
            {
                {"name", "Unity Tests"},
                {"total", summaryResults.TestsRun.ToString()},
                {"errors", summaryResults.Errors.ToString()},
                {"failures", summaryResults.Failures.ToString()},
                {"not-run", summaryResults.TestsNotRun.ToString()},
                {"inconclusive", summaryResults.Inconclusive.ToString()},
                {"ignored", summaryResults.Ignored.ToString()},
                {"skipped", summaryResults.Skipped.ToString()},
                {"invalid", summaryResults.NotRunnable.ToString()},
                {"date", now.ToString("yyyy-MM-dd")},
                {"time", now.ToString("HH:mm:ss")}
            }

            WriteOpeningElement("test-results", attributes);

            WriteEnvironment();
            WriteCultureInfo();
            WriteTestSuite(resultsName, summaryResults);
            WriteOpeningElement("results");
        }

        private void WriteOpeningElement(string elementName)
        {
            WriteOpeningElement(elementName, new Dictionary<string, string>());
        }

        private void WriteOpeningElement(string elementName, Dictionary<string, string> attributes)
        {
            WriteOpeningElement(elementName, attributes, false);
        }


        private void WriteOpeningElement(string elementName, Dictionary<string, string> attributes, bool closeImmediatelly)
        {
            WriteIndend();
            m_Indend++;
            m_ResultWriter.Append("<");
            m_ResultWriter.Append(elementName);
            foreach (var attribute in attributes)
            {
                m_ResultWriter.AppendFormat(" {0}=\"{1}\"", attribute.Key, SecurityElement.Escape(attribute.Value));
            }
            if (closeImmediatelly)
            {
                m_ResultWriter.Append(" /");
                m_Indend--;
            }
            m_ResultWriter.AppendLine(">");
        }

        private void WriteIndend()
        {
            for (int i = 0; i < m_Indend; i++)
            {
                m_ResultWriter.Append("  ");
            }
        }

        private void WriteClosingElement(string elementName)
        {
            m_Indend--;
            WriteIndend();
            m_ResultWriter.AppendLine("</" + elementName + ">");
        }

        private void WriteHeader()
        {
            m_ResultWriter.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            m_ResultWriter.AppendLine("<!--This file represents the results of running a test suite-->");
        }

        static string GetEnvironmentUserName()
        {
            #if !UNITY_WP8 && !UNITY_METRO
            return Environment.UserName;
            #else
            return "";
            #endif
        }

        static string GetEnvironmentMachineName()
        {
            #if !UNITY_WP8 && !UNITY_METRO
            return Environment.MachineName;
            #else
            return "";
            #endif
        }

        static string GetEnvironmentUserDomainName()
        {
            #if !UNITY_WP8 && !UNITY_METRO
            return Environment.UserDomainName;
            #else
            return "";
            #endif
        }

        static string GetEnvironmentVersion()
        {
            #if !UNITY_METRO
            return Environment.Version.ToString();
            #else
            return "";
            #endif
        }

        static string GetEnvironmentOSVersion()
        {
            #if !UNITY_METRO
            return Environment.OSVersion.ToString();
            #else
            return "";
            #endif
        }

        static string GetEnvironmentOSVersionPlatform()
        {
            #if !UNITY_METRO
            return Environment.OSVersion.Platform.ToString();
            #else
            return "";
            #endif
        }

        static string EnvironmentGetCurrentDirectory()
        {
            #if !UNITY_METRO
            return Environment.CurrentDirectory;
            #else
            return "";
            #endif
        }

        private void WriteEnvironment()
        {
            var attributes = new Dictionary<string, string>
            {
                {"nunit-version", k_NUnitVersion},
                {"clr-version", GetEnvironmentVersion()},
                {"os-version", GetEnvironmentOSVersion()},
                {"platform", GetEnvironmentOSVersionPlatform()},
                {"cwd", EnvironmentGetCurrentDirectory()},
                {"machine-name", GetEnvironmentMachineName()},
                {"user", GetEnvironmentUserName()},
                {"user-domain", GetEnvironmentUserDomainName()}
            }
            WriteOpeningElement("environment", attributes, true);
        }

        private void WriteCultureInfo()
        {
            var attributes = new Dictionary<string, string>
            {
                {"current-culture", CultureInfo.CurrentCulture.ToString()},
                {"current-uiculture", CultureInfo.CurrentUICulture.ToString()}
            }
            WriteOpeningElement("culture-info", attributes, true);
        }

        private void WriteTestSuite(string resultsName, ResultSummarizer summaryResults)
        {
            var attributes = new Dictionary<string, string>
            {
                {"name", resultsName},
                {"type", "Assembly"},
                {"executed", "True"},
                {"result", summaryResults.Success ? "Success" : "Failure"},
                {"success", summaryResults.Success ? "True" : "False"},
                {"time", summaryResults.Duration.ToString("#####0.000", NumberFormatInfo.InvariantInfo)}
            }
            WriteOpeningElement("test-suite", attributes);
        }

        private void WriteResultElement(ITestResult result)
        {
            StartTestElement(result);

            switch (result.ResultState)
            {
                case TestResultState.Ignored:
                case TestResultState.NotRunnable:
                case TestResultState.Skipped:
                    WriteReasonElement(result);
                    break;

                case TestResultState.Failure:
                case TestResultState.Error:
                case TestResultState.Cancelled:
                    WriteFailureElement(result);
                    break;
                case TestResultState.Success:
                case TestResultState.Inconclusive:
                    if (result.Message != null)
                        WriteReasonElement(result);
                    break;
            }

            WriteClosingElement("test-case");
        }

        private void TerminateXmlFile()
        {
            WriteClosingElement("results");
            WriteClosingElement("test-suite");
            WriteClosingElement("test-results");
        }

        #region Element Creation Helpers

        private void StartTestElement(ITestResult result)
        {
            var attributes = new Dictionary<string, string>
            {
                {"name", result.FullName},
                {"executed", result.Executed.ToString()}
            }
            string resultString;
            switch (result.ResultState)
            {
                case TestResultState.Cancelled:
                    resultString = TestResultState.Failure.ToString();
                    break;
                default:
                    resultString = result.ResultState.ToString();
                    break;
            }
            attributes.Add("result", resultString);
            if (result.Executed)
            {
                attributes.Add("success", result.IsSuccess.ToString());
                attributes.Add("time", result.Duration.ToString("#####0.000", NumberFormatInfo.InvariantInfo));
            }
            WriteOpeningElement("test-case", attributes);
        }

        private void WriteReasonElement(ITestResult result)
        {
            WriteOpeningElement("reason");
            WriteOpeningElement("message");
            WriteCData(result.Message);
            WriteClosingElement("message");
            WriteClosingElement("reason");
        }

        private void WriteFailureElement(ITestResult result)
        {
            WriteOpeningElement("failure");
            WriteOpeningElement("message");
            WriteCData(result.Message);
            WriteClosingElement("message");
            WriteOpeningElement("stack-trace");
            if (result.StackTrace != null)
                WriteCData(StackTraceFilter.Filter(result.StackTrace));
            WriteClosingElement("stack-trace");
            WriteClosingElement("failure");
        }

        #endregion

        private void WriteCData(string text)
        {
            if (text.Length == 0)
                return;
            m_ResultWriter.AppendFormat("<![CDATA[{0}]]>", text);
            m_ResultWriter.AppendLine();
        }

#if !UNITY_METRO
        public void WriteToFile(string resultDestiantion, string resultFileName)
        {
            try
            {
                var path = Path.Combine(resultDestiantion, resultFileName);
                Debug.Log("Saving results in " + path);
                using (var fs = File.OpenWrite(path))
                    using (var sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.Write(GetTestResult());
                    }
            }
            catch (Exception e)
            {
                Debug.LogError("Error while opening file");
                Debug.LogException(e);
            }
        }
#endif  // if !UNITY_METRO
    }
}
