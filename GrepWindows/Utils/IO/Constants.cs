using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smithgeek.IO
{
    internal static class Constants
    {
        public const int STANDARD_RETRY_ATTEMPTS = 10;
        public const int STANDARD_RETRY_SLEEP = 250;
        public const int USE_BUFFER_FILE_SIZE_BYTES = 1024 * 1024 * 10;
        public const int COPY_BUFFER_SIZE = 1024 * 1024 * 5;
    }
}
