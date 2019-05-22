using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xRetry.SpecFlow.Parsers
{
    public interface IRetryTagParser
    {
        RetryTag Parse(string tag);
    }
}
