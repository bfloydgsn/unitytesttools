using System;
using System.Collections.Generic;
using UnityEngine;
using UnityTest.IntegrationTestRunner;
#if !UNITY_METRO
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
#endif

namespace UnityTest
{
    public class NetworkResultSender : ITestRunnerCallback
    {
        private readonly TimeSpan m_ConnectionTimeout = TimeSpan.FromSeconds(5);

        private readonly string m_Ip;
        private readonly int m_Port;
        private bool m_LostConnection;

        public NetworkResultSender(string ip, int port)
        {
            m_Ip = ip;
            m_Port = port;
        }

        private bool SendDTO(ResultDTO dto)
        {
            if (m_LostConnection) return false;
#if !UNITY_METRO
            try
            {
                using (var tcpClient = new TcpClient())
                {
                    var result = tcpClient.BeginConnect(m_Ip, m_Port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(m_ConnectionTimeout);
                    if (!success)
                    {
                        return false;
                    }
                    try
                    {
                        tcpClient.EndConnect(result);
                    }
                    catch (SocketException)
                    {
                        m_LostConnection = true;
                        return false;
                    }

                    var bf = new BinaryFormatter();
                    bf.Serialize(tcpClient.GetStream(), dto);
                    tcpClient.GetStream().Close();
                    tcpClient.Close();
                    Debug.Log("Sent " + dto.messageType);
                }
            }
            catch (SocketException e)
            {
                Debug.LogException(e);
                m_LostConnection = true;
                return false;
            }
#endif  // if !UNITY_METRO
            return true;
        }

        public bool Ping()
        {
            var result = SendDTO(ResultDTO.CreatePing());
            m_LostConnection = false;
            return result;
        }

        public void RunStarted(string platform, List<TestComponent> testsToRun)
        {
            SendDTO(ResultDTO.CreateRunStarted());
        }

        public void RunFinished(List<TestResult> testResults)
        {
            SendDTO(ResultDTO.CreateRunFinished(testResults));
        }

        public void TestStarted(TestResult test)
        {
            SendDTO(ResultDTO.CreateTestStarted(test));
        }

        public void TestFinished(TestResult test)
        {
            SendDTO(ResultDTO.CreateTestFinished(test));
        }

        public void TestRunInterrupted(List<ITestComponent> testsNotRun)
        {
            RunFinished(new List<TestResult>());
        }
    }
}
