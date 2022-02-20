using NLog;


namespace ShutdownManager.Utility
{
    public class MyLogger : ILogger
    {

        //Only one instance of this class can be instanciated
        private static MyLogger instance; //singleton design pattern. singl instance of this class.
        private static Logger logger;//static variable to hold a single instance of the nLog logger.
        private const string rulesConfName = "myAppLoggerRules";

        //single design pattern - private constructor
        private MyLogger()
        {

        }


        //single design pattern shown here - this function creates an instance of the class if it has not ye been instancieated
        //exits int he programm, then send them the reference to the original.
        public static MyLogger GetInstance()
        {
            if(instance == null)
            {
                instance = new MyLogger();
            }
            return instance;
        }

        private Logger GetLogger(string theLogger)
        {
            if(logger == null)
            {
                MyLogger.logger = LogManager.GetLogger(theLogger);
            }
            return logger;
        }



        public void InfoWithClassName(string message, object nameOfClass, string arg = null)
        {

            Info("Class |" + nameOfClass.GetType().Name + "| " + message, arg);
        }
        public void DebugWithClassName(string message, object nameOfClass, string arg = null)
        {

            Debug("Class |" + nameOfClass.GetType().Name + "| " + message, arg);
        }
        public void WarningWithClassName(string message, object nameOfClass, string arg = null)
        {

            Warning("Class |" + nameOfClass.GetType().Name + "| " + message, arg);
        }
        public void ErrorWithClassName(string message, object nameOfClass, string arg = null)
        {

            Error("Class |" + nameOfClass.GetType().Name + "| " + message, arg);
        }


        public void Debug(string message, string arg = null)
        {
            if (arg == null)
                GetLogger(rulesConfName).Debug(message);
            else
                GetLogger(rulesConfName).Debug(message, arg);
        }

        public void Error(string message, string arg = null)
        {
            if (arg == null)
                GetLogger(rulesConfName).Error(message);
            else
                GetLogger(rulesConfName).Error(message, arg);
        }

        public void Info(string message, string arg = null)
        {
            if (arg == null)
                GetLogger(rulesConfName).Info(message);
            else
                GetLogger(rulesConfName).Info(message, arg);
        }

        public void Warning(string message, string arg = null)
        {
            if (arg == null)
                GetLogger(rulesConfName).Warn(message);
            else
                GetLogger(rulesConfName).Warn(message, arg);
        }
    }
}

