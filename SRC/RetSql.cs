using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace HttpService
{
    public class RetSql
    {
        public int status;
        public string message;
        public DataTable dataTable;

        public RetSql(int _status, string _message, DataTable _dataTable)
        {
            status = _status;
            message = _message;
            dataTable = _dataTable;
        }
    }
}
