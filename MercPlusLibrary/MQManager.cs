using System;
using IBM.WMQ;
using System.Text;

public class MQManager
{
    #region Constants
    private const string MQ_READ_MODE = "R";
    private const string MQ_WRITE_MODE = "W";
    #endregion Constants

    public MQQueueManager OpenQueueManager(string queueMgr)
    {
        MQQueueManager qMgr;
        try
        {
            qMgr = new MQQueueManager(queueMgr);
        }
        catch (MQException e)
        {
            //Error Message="The OpenQueueManager method failed.";
            //Error Message="Exception Msg: " + e.Message;	
            throw;
        }
        return qMgr;
    }

    public MQQueue OpenQ(string mode, MQQueueManager qMgr, string queue)
    {
        MQQueue Q;
        try
        {
            if (mode == MQ_READ_MODE)
            {
                Q = qMgr.AccessQueue(queue, MQC.MQOO_BROWSE | MQC.MQOO_INPUT_AS_Q_DEF | MQC.MQOO_FAIL_IF_QUIESCING | MQC.MQOO_INQUIRE);
            }
            else
            {
                Q = qMgr.AccessQueue(queue, MQC.MQOO_OUTPUT | MQC.MQOO_FAIL_IF_QUIESCING | MQC.MQOO_INQUIRE);
            }
        }
        catch (MQException e)
        {
            //ErrorMessage="The OpenQ method failed.";
            //ErrorMessage="Exception Msg: " + e.Message;	
            throw;
        }
        return Q;
    }

    public void CloseQ(MQQueue Q)
    {
        try
        {
            Q.Close();
        }
        catch (MQException e)
        {
            //ErrorMessage="The CloseQ method failed.";
            //ErrorMessage="Exception Msg: " + e.Message;	
            throw;
        }
    }

    public void DisconnectQueueManager(IBM.WMQ.MQQueueManager QMgr)
    {
        try
        {
            QMgr.Disconnect();
        }
        catch (MQException e)
        {
            //ErrorMessage="The DisconnectQueueManager method failed.";
            //ErrorMessage="Exception Msg: " + e.Message;	
            throw;
        }
    }

    public string GetMessage(IBM.WMQ.MQQueue Q, bool nonDestructiveRead)
    {
        string msg = "";
        try
        {
            IBM.WMQ.MQMessage outMess = new IBM.WMQ.MQMessage();
            IBM.WMQ.MQGetMessageOptions getOptions = new IBM.WMQ.MQGetMessageOptions();
            getOptions.WaitInterval = 5000;
            getOptions.Options = MQC.MQGMO_WAIT | MQC.MQOO_FAIL_IF_QUIESCING;
            if (nonDestructiveRead)
            {
                getOptions.Options = MQC.MQGMO_BROWSE_NEXT | MQC.MQGMO_WAIT;
            }
            Q.Get(outMess, getOptions);
            msg = outMess.ReadString(outMess.MessageLength);
        }
        catch (IBM.WMQ.MQException e)
        {
            //ErrorMessage="The GetMessage method failed.";
            //ErrorMessage="Exception Msg: " + e.Message;	
            throw;
        }
        return msg;
    }

    public void GetMessage(IBM.WMQ.MQQueue Q, ref IBM.WMQ.MQMessage outMess, bool nonDestructiveRead)
    {
        try
        {
            IBM.WMQ.MQGetMessageOptions getOptions = new IBM.WMQ.MQGetMessageOptions();
            getOptions.WaitInterval = 5000;
            getOptions.Options = MQC.MQGMO_WAIT | MQC.MQOO_FAIL_IF_QUIESCING;
            if (nonDestructiveRead)
            {
                getOptions.Options = MQC.MQGMO_BROWSE_NEXT | MQC.MQGMO_WAIT;
            }
            Q.Get(outMess, getOptions);
        }
        catch (IBM.WMQ.MQException e)
        {
            //ErrorMessage="The GetMessage method failed.";
            //ErrorMessage="Exception Msg: " + e.Message;	
            throw;
        }
    }

    public string GetMessage(IBM.WMQ.MQQueue Q, bool nonDestructiveRead, byte[] corrId)
    {
        string msg = "";
        try
        {
            IBM.WMQ.MQMessage outMess = new IBM.WMQ.MQMessage();
            IBM.WMQ.MQGetMessageOptions getOptions = new IBM.WMQ.MQGetMessageOptions();
            getOptions.MatchOptions = MQC.MQMO_MATCH_CORREL_ID;
            getOptions.WaitInterval = 5000;
            outMess.CorrelationId = corrId;
            getOptions.Options = MQC.MQGMO_WAIT | MQC.MQOO_FAIL_IF_QUIESCING;
            if (nonDestructiveRead)
            {
                getOptions.Options = MQC.MQGMO_BROWSE_NEXT | MQC.MQGMO_WAIT;
            }
            Q.Get(outMess, getOptions);
            msg = outMess.ReadString(outMess.MessageLength);
            //corrId=ConvertByteArray2String(outMess.CorrelationId);			
        }
        catch (IBM.WMQ.MQException e)
        {
            //ErrorMessage="The GetMessage method failed.";
            //ErrorMessage="Exception Msg: " + e.Message;	
            throw;
        }
        return msg;
    }

    public string GetMessage(IBM.WMQ.MQQueue Q, bool nonDestructiveRead, ref string corrId)
    {
        string msg = "";
        try
        {
            IBM.WMQ.MQMessage outMess = new IBM.WMQ.MQMessage();
            IBM.WMQ.MQGetMessageOptions getOptions = new IBM.WMQ.MQGetMessageOptions();
            getOptions.WaitInterval = 5000;
            getOptions.Options = MQC.MQGMO_WAIT | MQC.MQOO_FAIL_IF_QUIESCING;
            if (nonDestructiveRead)
            {
                getOptions.Options = MQC.MQGMO_BROWSE_NEXT | MQC.MQGMO_WAIT;
            }
            Q.Get(outMess, getOptions);
            msg = outMess.ReadString(outMess.MessageLength);
            corrId = ConvertByteArray2String(outMess.CorrelationId);
        }
        catch (IBM.WMQ.MQException e)
        {
            //ErrorMessage="The GetMessage method failed.";
            //ErrorMessage="Exception Msg: " + e.Message;	
            throw;
        }
        return msg;
    }

    public string GetMessage(IBM.WMQ.MQQueue Q, bool nonDestructiveRead, out byte[] msgId)
    {
        string msg = "";
        try
        {
            IBM.WMQ.MQMessage outMess = new IBM.WMQ.MQMessage();
            IBM.WMQ.MQGetMessageOptions getOptions = new IBM.WMQ.MQGetMessageOptions();
            getOptions.WaitInterval = 5000;
            getOptions.Options = MQC.MQGMO_WAIT | MQC.MQOO_FAIL_IF_QUIESCING;
            if (nonDestructiveRead)
            {
                getOptions.Options = MQC.MQGMO_BROWSE_NEXT | MQC.MQGMO_WAIT;
            }
            Q.Get(outMess, getOptions);
            msg = outMess.ReadString(outMess.MessageLength);
            msgId = outMess.MessageId;
        }
        catch (IBM.WMQ.MQException e)
        {
            //ErrorMessage="The GetMessage method failed.";
            //ErrorMessage="Exception Msg: " + e.Message;	
            throw;
        }
        return msg;
    }

    public int GetMessageCount(MQQueue Q)
    {
        try
        {
            return Q.CurrentDepth;
        }
        catch (MQException e)
        {
            //ErrorMessage="The GetMessageCount method failed.";
            //ErrorMessage="Exception Msg: " + e.Message;	
            throw;
        }
    }

    private string ConvertByteArray2String(byte[] data)
    {
        string s = "";
        UnicodeEncoding unicode = new UnicodeEncoding();
        ASCIIEncoding ascii = new ASCIIEncoding();
        char[] ch = ascii.GetChars(data);
        s = new string(ch);
        return s;
    }

    public bool PutMessage(MQQueue Q, string message)
    {
        bool bRet = false;
        try
        {
            MQMessage msg = new MQMessage();
            MQPutMessageOptions putOptions = new MQPutMessageOptions();
            msg.WriteString(message);
            //msg.CharacterSet=850;
            //msg.Encoding=273;
            msg.Format = MQC.MQFMT_STRING;
            Q.Put(msg);
            bRet = true;

        }
        catch (MQException e)
        {
            //ErrorMessage="The PutMessage method failed.";
            //ErrorMessage="Exception Msg: " + e.Message;	
            //throw;
        }
        return bRet;
    }

    public bool PutMessage(MQQueue Q, string message, string replyQMGR, string replyQ, ref byte[] messageID)
    {
        bool bRet = false;
        try
        {
            MQMessage msg = new MQMessage();
            MQPutMessageOptions putOptions = new MQPutMessageOptions();
            msg.WriteString(message);
            msg.ReplyToQueueManagerName = replyQMGR;
            msg.ReplyToQueueName = replyQ;
            //msg.CorrelationId=corrId;
            msg.Format = MQC.MQFMT_STRING;
            Q.Put(msg);
            bRet = true;
            messageID = msg.MessageId;
        }
        catch (MQException e)
        {
            //ErrorMessage="The PutMessage method failed.";
            //ErrorMessage="Exception Msg: " + e.Message;	
            //throw;
        }
        return bRet;
    }

    public bool PutMessage(MQQueue Q, string message, string replyQMGR, string replyQ, ref string messageID)
    {
        bool bRet = false;
        try
        {
            MQMessage msg = new MQMessage();
            MQPutMessageOptions putOptions = new MQPutMessageOptions();
            msg.WriteString(message);
            msg.ReplyToQueueManagerName = replyQMGR;
            msg.ReplyToQueueName = replyQ;
            //msg.CorrelationId=corrId;
            msg.Format = MQC.MQFMT_STRING;
            Q.Put(msg);
            bRet = true;
            messageID = ConvertByteArray2String(msg.MessageId);
        }
        catch (MQException e)
        {
            //ErrorMessage="The PutMessage method failed.";
            //ErrorMessage="Exception Msg: " + e.Message;	
            //throw;
        }
        return bRet;
    }
    public bool PutMessageWithOptions(MQQueue Q, MQMessage msg, MQPutMessageOptions putOptions)
    {
        bool bRet = false;
        try
        {
            Q.Put(msg, putOptions);
            bRet = true;
        }
        catch (MQException e)
        {
            //ErrorMessage="The PutMessage method failed.";
            //ErrorMessage="Exception Msg: " + e.Message;	
            throw;
        }
        return bRet;
    }

    public MQQueue OpenRemoteQueueWithContext(MQQueueManager qMgr, string queue)
    {
        MQQueue Q;
        try
        {
            Q = qMgr.AccessQueue(queue, MQC.MQOO_OUTPUT | MQC.MQOO_FAIL_IF_QUIESCING | MQC.MQOO_INQUIRE | MQC.MQOO_SET_ALL_CONTEXT);

        }
        catch (MQException e)
        {
            //ErrorMessage="The OpenQ method failed.";
            //ErrorMessage="Exception Msg: " + e.Message;	
            throw;
        }
        return Q;
    }


}
