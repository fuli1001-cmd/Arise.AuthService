using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileService.File.API.Settings
{
    public class CleanSettings
    {
        // 清理开始小时
        public int StartHour { get; set; }

        // 清理开始分钟
        public int StartMinute { get; set; }

        // 清理间隔小时数
        public int IntervalHours { get; set; }

        public int DelaySeconds { get; set; }
    }

    // 程序内部文件清理设置
    public class AppCleanSettings : CleanSettings { }

    // 聊天文件清理设置
    public class ChatCleanSettings: CleanSettings { }
}
