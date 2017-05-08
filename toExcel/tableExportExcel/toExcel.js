var idTmr;
function getExplorer() {
    var explorer = window.navigator.userAgent;
    if (explorer.indexOf("MSIE") >= 0) {
        return 'ie';
    }
    else if (explorer.indexOf("Firefox") >= 0) {
        return 'Firefox';
    }
    else if (explorer.indexOf("Chrome") >= 0) {
        return 'Chrome';
    }
    else if (explorer.indexOf("Opera") >= 0) {
        return 'Opera';
    }
    else if (explorer.indexOf("Safari") >= 0) {
        return 'Safari';
    }
}

//导出方法入口
function exportExcel(tableid) {
    if (getExplorer() == 'ie') {
        var curTbl = document.getElementById(tableid);
        var oXL;
        try {
            oXL = new ActiveXObject("Excel.Application"); //创建AX对象excel  
        } catch (e) {
            alert("无法启动Excel!\n\n如果您确信您的电脑中已经安装了Excel，" + "那么请调整IE的安全级别。\n\n具体操作：\n\n" + "工具 → Internet选项 → 安全 → 自定义级别 → 对没有标记为安全的ActiveX进行初始化和脚本运行 → 启用");
            return false;
        }
        var oWB = oXL.Workbooks.Add(); //获取workbook对象  
        var oSheet = oWB.ActiveSheet;//激活当前sheet  
        var sel = document.body.createTextRange();
        sel.moveToElementText(curTbl); //把表格中的内容移到TextRange中  
        sel.select(); //全选TextRange中内容  
        sel.execCommand("Copy");//复制TextRange中内容  
        oSheet.Paste();//粘贴到活动的EXCEL中  
        oXL.Visible = true; //设置excel可见属性  
        var fname = oXL.Application.GetSaveAsFilename("将table导出到excel.xls", "Excel Spreadsheets (*.xls), *.xls");
        oWB.SaveAs(fname);
        oWB.Close();
        oXL.Quit();
    }
    else {
        tableToExcel(tableid);
    }
}

//除ie导出的excel的方法
var tableToExcel = (function () {
    var uri = 'data:application/vnd.ms-excel;base64,',
    template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel"' +
    'xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet>'
    + '<x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets>'
    + '</x:ExcelWorkbook></xml><![endif]-->' +
    ' <style type="text/css">' +
    '.excelTable  {' +
    'border-collapse:collapse;' +
     ' border:thin solid #999; ' +
    '}' +
    '   .excelTable  th {' +
    '   border: thin solid #999;' +
    '  padding:20px;' +
    '  text-align: center;' +
    '  border-top: thin solid #999;' +
    ' background-color: #E6E6E6;' +
    ' }' +
    ' .excelTable  td{' +
    ' border:thin solid #999;' +
    '  padding:2px 5px;' +
    '  text-align: center;' +
    ' }</style>' +
    '</head><body ><table class="excelTable">{table}</table></body></html>',
    base64 = function (s) { return window.btoa(unescape(encodeURIComponent(s))); },
    format = function (s, c) {
        return s.replace(/{(\w+)}/g,
        function (m, p) {
            return c[p]
        })
    };
    return function (table, name) {
        if (!table.nodeType) table = document.getElementById(table);
        var ctx = { worksheet: name || 'Worksheet', table: table.innerHTML };
        //window.location.href = uri + base64(format(template, ctx))  ;
        document.getElementById("exportExcel").href = uri + base64(format(template, ctx));
        document.getElementById("exportExcel").download = '告警详情';
        //document.getElementById("exportExcel").click();
    };
})();
function Cleanup() {
    window.clearInterval(idTmr);
    CollectGarbage();
}