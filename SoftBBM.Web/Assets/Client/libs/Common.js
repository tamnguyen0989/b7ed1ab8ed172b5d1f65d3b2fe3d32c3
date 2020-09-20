var Common = {
    RowIndex: 0,
    GMT: "+0700"
};

function createJSCSSFile(filename, filetype) {
    if (checkExistJSCSSFile(filename, filetype)) return;
    var fileref = null;
    var suffix = "?v=" + document.getElementById("CMIS_BUILD_VERSION").value;
    if (filetype == "js") { //if filename is a external JavaScript file
        fileref = document.createElement('script')
        fileref.setAttribute("type", "text/javascript")
        fileref.setAttribute("src", filename + suffix)
    }
    else if (filetype == "css" || filetype == "css_print") { //if filename is an external CSS file
        fileref = document.createElement("link")
        fileref.setAttribute("rel", "stylesheet")
        fileref.setAttribute("type", "text/css")
        fileref.setAttribute("href", filename + suffix)
        if (filetype == "css_print") fileref.setAttribute("media", "print")
    }
    if (!isNullOrEmpty(fileref)) {
        document.getElementsByTagName("head")[0].appendChild(fileref)
    }
    return fileref
}

function checkExistJSCSSFile(filename, filetype) {
    var targetelement = (filetype == "js") ? "script" : (filetype == "css") ? "link" : "none" //determine element type to create nodelist using
    var targetattr = (filetype == "js") ? "src" : (filetype == "css") ? "href" : "none" //determine corresponding attribute to test for
    var allsuspects = document.getElementsByTagName(targetelement)
    for (var i = allsuspects.length; i >= 0; i--) { //search backwards within nodelist for matching elements to remove
        if (allsuspects[i] && allsuspects[i].getAttribute(targetattr) != null
            && allsuspects[i].getAttribute(targetattr).indexOf(filename) != -1) {
            return true;
            //var newelement = createjscssfile(newfilename, filetype)
            //allsuspects[i].parentNode.replaceChild(newelement, allsuspects[i])
        }
    }
    return false;
}

function sortObject(list, prop) {
    return list.sort(function (item1, item2) { return item1[prop] < item2[prop] ? -1 : (item1[prop] > item2[prop] ? 1 : 0) });
}

function validateEmpty(items, props, msg) {
    var isValid = true;
    if (Array.isArray(items)) {
        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            var index = 0;
            for (var prop in props) {
                var elementInput = document.getElementById(props[prop] + "_" + i);
                isValid = validateElement(elementInput);
                if (!isValid) {
                    notification("Vui lòng " + msg[index], "error");
                    return isValid;
                }
                index++;
            }
        }
    }
    else {
        var index = 0;
        for (var prop in props) {
            var elementInput = document.getElementById(props[prop]);
            isValid = validateElement(elementInput);
            if (!isValid) {
                notification("Vui lòng " + msg[index], "error");
                return isValid;
            }
            index++;
        }
    }
    return isValid;
}

function validateElement(element) {
    if (isNullOrEmpty(element)) {
        return false;
    }
    else if (typeof element.value === 'string') {
        if (isNullOrEmpty(element.value.replace(/\s/g, ''))) {
            focusElement(element);
            return false;
        }
    }
    else if (isNullOrEmpty(element.value)) {
        focusElement(element);
        return false;
    }
    return true;
}

function focusElement(element) {
    try { element.focus(); } catch (e) { }
    try { element.previousSibling.focus(); } catch (e) { }
    try { element.previousSibling.firstChild.focus(); } catch (e) { }
}

function focusElementV2(element) {
    try { element.focus(); } catch (e) { }
    try { element.input.focus(); } catch (e) { }
}

function validateDateElement(moment, element) {
    var date = formatStringtoDate(moment, document.getElementById(element).value);
    if (isNullOrEmpty(date)) {
        document.getElementById(element).value = null;
        return false;
    }
    return true;
}

function formatStringtoDate(moment, dateString) {
    if (isNullOrEmpty(dateString)) return null;
    var date = moment(dateString, "DD/MM/YYYY", true);
    return !date.isValid() ? null : new Date(date);
}

function getRowIndexNext() {
    Common.RowIndex++;
    return Common.RowIndex;
}

function replaceObj(obj, value, objName) {
    for (var name in obj) {
        var search = objName + "." + name;
        if (typeof (obj[name]) !== 'object') {
            value = value.split(search).join(obj[name]);//.replace(search, obj[name]);
        }
        else {
            value = ReplaceObj(obj[name], value, search);
        }
    }
    return value;
}

function setRowIndex(index) {
    Common.RowIndex = parseInt(index);
}

function redirectPage(url) {
    window.location.href = url;
}

function redirectPageBlank(url) {
    window.open(url, '_blank');
}

function openPage(url) {
    window.open(url);
}

function redirectPageMetatag(url) {
    //window.location.href = url;
}

function reloadPage() {
    window.location.reload();
}

function goBack() {
    window.history.go(-1);
}

function isNullOrEmpty(data) {
    if (data === null) {
        return true;
    }
    if (typeof (data) === 'undefined') {
        return true;
    }
    if (data === "") {
        return true;
    }
    if (data === undefined) {
        return true;
    }
    return false;
}

function isNullOrWhiteSpace(data) {
    if (isNullOrEmpty(data)) {
        return true;
    }
    else {
        if (typeof data === 'string') {
            data = data.replace(" ", "");
        }
        return isNullOrEmpty(data);
    }

}

function createDateTimePicker(controlID, format) {
    var control = $("#" + controlID).kendoDatePicker({
        animation: false,
        format: format
    });
    return control;
}
function notification(mess, typeMess) {
    $.niftyNoty({
        type: (typeMess == 'error') ? 'danger' : typeMess,
        container: 'floating',
        html: mess,
        timer: 3000
    });
}
function pageNotification(mess, typeMess) {
    $.niftyNoty({
        type: typeMess,
        container: 'page',
        html: mess,
        timer: 22400
    });
}
function dateCompare(source, destination) {
    var month = source.getMonth() >= 10 ? source.getMonth() : "0" + source.getMonth();
    var date = source.getDate() >= 10 ? source.getDate() : "0" + source.getDate();
    var sourceDate = "" + source.getFullYear() + "" + month + "" + date;

    month = destination.getMonth() >= 10 ? destination.getMonth() : "0" + destination.getMonth();
    date = destination.getDate() >= 10 ? destination.getDate() : "0" + destination.getDate();
    var destinationDate = "" + destination.getFullYear() + "" + month + "" + date;

    sourceDate = parseInt(sourceDate);
    destinationDate = parseInt(destinationDate);
    var comparer = sourceDate - destinationDate;
    if (comparer == 0) {
        return 0;
    }
    else if (comparer > 0) {
        return 1;
    }
    else {
        return -1;
    }
}

function dateTimeCompare(date1, date2) {
    var date1_ms = date1.getTime();
    var date2_ms = date2.getTime();
    var difference_ms = date2_ms - date1_ms;
    return difference_ms;
}

function getUrlByDocumentType(documentType, dataItem) {
    documentType = parseInt(documentType);
    //console.log("documemt: ", documentType);
    var url = "";
    /*
    Opportunity = 1,
        Advance,
        Clearing,
        Payment,
        BuyContract,
        SaleContract,
        Customer,
        Contract,
        BusinessPlan,
        Estimate,
        Consultant,
        M,
        N,
        O,
        P,
        X,
        Unknown
    */
    switch (documentType) {
        case 1:
            {
                url = "#/opportunity/overview/";
                break;
            }
        case 2:
            {
                url = "#/thongtinphieutamung/";
                break;
            }
        case 3:
            {
                url = "#/clearing/edit/";
                break;
            }
        case 4:
            {
                url = "#/thongtinphieuthanhtoan/";
                break;
            }
        case 7:
            {
                if (dataItem != null && dataItem.Document != null && dataItem.Document.CustomerSupplier == 1) {
                    url = "#/thongtinkhachhang/";
                }
                else if (dataItem != null && dataItem.Document != null && dataItem.Document.CustomerSupplier == 2) {
                    url = "#/thongtinnhacungcap/";
                }
                break;
            }
        case 8:
            {
                url = "#/hopdong/";
                break;
            }
        case 9:
            {
                url = "#/phuongankinhdoanh/";
                break;
            }
        case 10:
            {
                url = "#/business/estimate/";
                break;
            }
        case 11:
            {
                url = "#/yeucautuvan/";
                break;
            }
        case 12:
            {
                url = "#/business/assignment/";
                break;
            }
        case 21:
            {
                url = "#/thongtincohoikinhdoanh/";
                break;
            }
        case 24:
            {
                url = "#/registervacation/";
                break;
            }
        case 26:
            {
                url = "#/dexuatkhachhang/";
                break;
            }
        case 27:
            {
                url = "#/estimatecontract/edit/";
                break;
            }
        case 28:
            {
                url = "#/finalizationcontract/edit/";
                break;
            }
        case 29:
            {
                url = "#/business/biddocument/";
                break;
            }
        case 30:
            {
                url = "#/requestexecutive/edit/";
                break;
            }
        case 33:
            {
                url = "#/registerroom/";
                break;
            }
        case 34:
            {
                url = "#/finalizationestimate/edit/";
                break;
            }
        case 36:
            {
                url = "#/business/estimateguarantee/";
                break;
            }
        case 37:
            {
                url = "#/registerservice/";
                break;
            }
        case 38:
            {
                url = "#/carriage/";
                break;
            }
        case 39:
            {
                url = "#/documentary/";
                break;
            }
        case 40:
            {
                url = "#/overtime/";
                break;
            }
        case 41:
            {
                if (dataItem != null && dataItem.Document != null && dataItem.Document.Type == 0) {
                    url = "#/registerpropose/";
                }
                else if (dataItem != null && dataItem.Document != null && dataItem.Document.Type == 1) {
                    url = "#/registerproposerecord/";
                }
                break;
            }
        case 51:
            {
                url = "#/yeucaumokhoamvv/";
                break;
            }
        case 54:
            {
                url = "#/chitietrebate/";
                break;
            }
        case 58:
            {
                url = "#/yeucauxuathang/";
                break;
            }
        case 59:
            {
                url = "#/yeucaumuonhang/";
                break;
            }
        case 60:
            {
                url = "#/yeucaudieuchuyenhang/";
                break;
            }
        case 62:
            {
                url = "#/thongtinbaogiadichvu/";
                break;
            }
        case 64:
            {
                url = "#/dexuatcongtac/";
                break;
            }
        case 67:
            {
                url = "#/xemxethopdong/";
                break;
            }
        case 68:
            {
                url = "#/changerequest/edit/";
                break;
            }
        case 69:
            {
                url = "#/business/requestopencancelopportunity/";
                break;
            }
        case 71:
            {
                url = "#/MassPayment/";
                break;
            }
        case 73:
            {
                url = "#/danhgiachitietungvien/";
                break;
            }
        case 74:
            {
                url = "#/thongtinyeucautuyendung/";
                break;
            }
        case 75:
            {
                url = "#/BorrowBook/";
                break;
            }
        case 77:
            {
                url = "#/baocaothuviec/";
                break;
            }
        case 78:
            {
                url = "#/donxinthoiviec/";
                break;
            }
        case 83:
            {
                url = "#/bangiaolienbophan/";
                break;
            }
        case 84:
            {
                url = "#/bangiaocongviec/";
                break;
            }
        case 85:
            {
                url = "#/thongtindexuattuyendung/";
                break;
            }
        case 87:
            {
                url = "#/yeucaudieuchuyenhangton/";
                break;
            }
        case 90:
            {
                url = "#/yeucaubanhangtonkho/";
                break;
            }
        case 93:
            {
                url = "#/approvetimekeeping/";
                break;
            }
        case 97:
            {
                url = "#/InfoReviewExpiredContract/";
                break;
            }
        case 104:
            {
                url = "#/thamdinhtindungkhachhang/";
                break;
            }
        case 106:
            {

                url = "#/kehoachtieuthu/";
                break;
            }
        case 107:
            {
                if (dataItem != null && dataItem.Document != null && dataItem.Document.Type == 3) {
                    url = "#/kehoachtieuthutonghopduan/";
                }
                else {
                    url = "#/kehoachtieuthutonghopphanphoi/";
                }
                break;
            }
        case 108:
            {
                url = "#/chinhsachgiaphanphoi/";
                break;
            }
        case 110:
            {
                url = "#/donhang/";
                break;
            }
        //Requestpurchase
        case 111:
            {
                url = "#/yeucaumuahang/";
                //url ="#/dieuchinhphuonganmuahang/"
                break;
            }
        case 113:
            {
                url = "#/phuonganmuahang/";
                break;
            }
        case 119:
            {
                url = "#/overtime/";
                break;
            }
        case 121:
            {
                url = "#/dieuchinhquyettoanduan/";
                break;
            }
        case 114:
            {
                url = "#/kehoachmuahanguyquyenasp/";
                break;
            }
        case 123:
            {
                url = "#/quanlygiaonhanviec/";
                break;
            }
        case 124:
            {
                url = "#/dieuchinhdanhgiathamdo/";
                break;
            }
        case 125:
            {
                url = "#/dieuchinhquyettoanphanphoi/";
                break;
            }
        case 126:
            {
                url = "#/dieuchinhquyettoandichvu/";
                break;
            }
        case 115:
            {
                url = "#/yeucausanxuat/";
                break;
            }
        case 128:
            {
                url = "#/danhsachvattumoi/";
                break;
            }
    }
    return url;

}

function getUrlByDataItem(dataItem) {
    if (dataItem.Document == null) return "";
    var obj = dataItem;
    var url = "";
    switch (obj.DocumentType) {
        case 2: {
            var doc = dataItem.Document;
            url += doc.OpportunityID + "/" + doc.ID;
            break;
        }
        case 3: {
            var doc = dataItem.Document;
            url += doc.OpportunityID + "/" + doc.AdvanceID + "/" + doc.ID;
            break;
        }
        case 4: {
            var doc = dataItem.Document;
            url += doc.OpportunityID + "/" + doc.ID;
            break;
        }
        case 7:
            {
                var doc = dataItem.Document;
                url += doc.ID;
                break;
            }
        case 8:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 9:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 10:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.EstimateType + "/" + (doc.IsDetail ? 1 : 0) + "/" + doc.ID;
                break;
            }
        case 11:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 21:
            {
                var doc = dataItem.Document;
                url += doc.FormType + "/" + doc.ID;
                break;
            }
        case 24:
            {
                var doc = dataItem.Document;
                url += doc.ID;
                break;
            }
        case 26:
            {
                var doc = dataItem.Document;
                url += doc.ID;
                break;
            }
        case 27:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID + "/" + doc.Version;
                break;
            }
        case 28:
            {
                var doc = dataItem.Document;
                url += doc.FinalizationContractType + "/" + doc.OpportunityID + "/" + doc.ID + "/" + doc.Version;
                break;
            }
        case 29:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 30:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 33:
            {
                var doc = dataItem.Document;
                url += doc.ID;
                break;
            }
        case 34:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 36:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 37:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 38:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 39:
            {
                var doc = dataItem.Document;
                url += (doc.OpportunityID > 0 ? doc.OpportunityID : 0) + "/" + doc.ID;
                break;
            }
        case 40:
            {
                var doc = dataItem.Document;
                url += doc.ID;
                break;
            }
        case 41:
            {
                var doc = dataItem.Document;
                url += doc.ID;
                break;
            }
        case 51:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 54:
            {
                var doc = dataItem.Document;
                url += doc.ID;
                break;
            }
        case 58:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 59:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 60:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 62:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 64:
            {
                var doc = dataItem.Document;
                url += (doc.OpportunityID > 0 ? doc.OpportunityID : 0) + "/" + doc.ID;
                break;
            }
        case 67:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 68:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ChangeRequestDocumentID + "/" + doc.DocumentID + "/" + doc.ID;
                break;
            }
        case 69:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.RequestOCOpportunity + "/" + doc.ID;
                break;
            }
        case 71:
            {
                var doc = dataItem.Document;
                url += doc.ID;
                break;
            }
        case 73:
            {
                var doc = dataItem.Document;
                url += doc.HRCandidateID + "/" + doc.ID;
                break;
            }
        case 74:
            {
                var doc = dataItem.Document;
                url += doc.ID;
                break;
            }
        case 75:
            {
                var doc = dataItem.Document;
                url += doc.ID;
                break;
            }
        case 77:
            {
                var doc = dataItem.Document;
                url += doc.TestWorkPlanID + "/" + doc.ID;
                break;
            }
        case 78:
            {
                var doc = dataItem.Document;
                url += doc.ID;
                break;
            }
        case 80:
            {
                var doc = dataItem.Document;
                url += doc.ID;
                break;
            }
        case 83:
            {
                var doc = dataItem.Document;
                url += doc.ID;
                break;
            }
        case 84:
            {
                var doc = dataItem.Document;
                url += doc.ID;
                break;
            }
        case 85:
            {
                var doc = dataItem.Document;
                url += doc.ID;
                break;
            }
        case 87:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 90:
            {
                var doc = dataItem.Document;
                url += doc.ID;
                break;
            }
        case 97:
            {
                var doc = dataItem.Document;
                url += doc.ID;
                break;
            }
        case 104:
            {
                var doc = dataItem.Document;
                url += doc.ID;
                break;
            }
        case 106:
            {

                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 107:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 108:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 110:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 111:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                //url += doc.ID + "/";
                break;
            }
        case 113:
            {
                var doc = dataItem.Document;
                url += doc.ID;
                break;
            }
        case 119:
            {
                var doc = dataItem.Document;
                url += doc.ID;
                break;
            }
        case 121:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 114:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 123:
            {
                var doc = dataItem.Document;
                url += doc.Status;
                break;
            }
        case 124:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 125:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 126:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 115:
            {
                var doc = dataItem.Document;
                url += doc.OpportunityID + "/" + doc.ID;
                break;
            }
        case 128:
            {
                var doc = dataItem.Document;
                url += doc.Code;
                break;
            }
    }
    return url;
}

function buildWorkFlowJS() {
    var html = "";
    html += ('<script src="/js/lib/jsplumb/external/jquery-ui-1.9.2.min.js"></script>');
    html += ('<script src="/js/lib/jsplumb/external/jquery.ui.touch-punch-0.2.2.min.js"></script>');
    html += ('<script src="/js/lib/jsplumb/lib/jsBezier-0.6.js"></script>');
    html += ('<script src="/js/lib/jsplumb/lib/mottle-0.4.js"></script>');
    html += ('<script src="/js/lib/jsplumb/lib/biltong-0.2.js"></script>');
    html += ('<script src="/js/lib/jsplumb/src/util.js"></script>');
    html += ('<script src="/js/lib/jsplumb/src/browser-util.js"></script>');
    html += ('<script src="/js/lib/jsplumb/src/dom-adapter.js"></script>');
    html += ('<script src="/js/lib/jsplumb/src/jsPlumb.js"></script>');
    html += ('<script src="/js/lib/jsplumb/src/endpoint.js"></script>');
    html += ('<script src="/js/lib/jsplumb/src/connection.js"></script>');
    html += ('<script src="/js/lib/jsplumb/src/anchors.js"></script>');
    html += ('<script src="/js/lib/jsplumb/src/defaults.js"></script>');
    html += ('<script src="/js/lib/jsplumb/src/connectors-bezier.js"></script>');
    html += ('<script src="/js/lib/jsplumb/src/connectors-statemachine.js"></script>');
    html += ('<script src="/js/lib/jsplumb/src/connectors-flowchart.js"></script>');
    html += ('<script src="/js/lib/jsplumb/src/connector-editors.js"></script>');
    html += ('<script src="/js/lib/jsplumb/src/renderers-svg.js"></script>');
    html += ('<script src="/js/lib/jsplumb/src/renderers-vml.js"></script>');
    html += ('<script src="/js/lib/jsplumb/src/base-library-adapter.js"></script>');
    html += ('<script src="/js/lib/jsplumb/src/jquery.jsPlumb.js"></script>');
    html += ('');
    html += ('');
    html += ('');
    html += ('');
    html += ('');

    return html;
    //$(document).append(html);
}

function splitControllElement(elementText) {
    /*
    input: string
    output: array[key, value];
    */
    var result = new Array();
    var elements = elementText.split('\n');
    if (elements.length > 0) {
        for (var i = 0; i < elements.length; i++) {
            var keyValuePair = elements[i];
            if (keyValuePair.indexOf(';#')) {
                var keyValue = keyValuePair.split(';#');
                result.push({ Value: keyValue[0], Name: keyValue[1] });
            }
            else {
                result.push({ Value: keyValuePair, Name: keyValuePair });
            }
        }
    }
    return result;
}

function dateDiff(date1, date2) {
    if (isNullOrEmpty(date1) || isNullOrEmpty(date2)) {
        console.log("date wrong format");
        return;
    }
    else {
        var one_day = 1000 * 60 * 60 * 24;

        // Convert both dates to milliseconds
        var date1_ms = date1.getTime();
        var date2_ms = date2.getTime();
        var difference_ms = date2_ms - date1_ms;
        // Convert back to days and return
        return Math.round(difference_ms / one_day);
    }
}

function getTimeByMinute(date1, date2) {

    var one_Minute = 1000 * 60;
    // Convert both dates to milliseconds
    var date1_ms = date1.getTime();
    var date2_ms = date2.getTime();
    var difference_ms = date2_ms - date1_ms;
    // Convert back to days and return
    return Math.round(difference_ms / one_Minute);
}

function dateDiffAdvance(date1, date2) {
    var one_day = 1000 * 60 * 60 * 24;

    // Convert both dates to milliseconds
    var date1_ms = date1.getTime();
    var date2_ms = date2.getTime();
    var difference_ms = date2_ms - date1_ms;
    if (difference_ms < 0) return -1;
    // Convert back to days and return
    return Math.round(difference_ms / one_day);
}


function formatDate(dateTime) {
    var day = new Date(dateTime);
    var month = day.getMonth();
    var date = day.getDate();
    if (month < 10)
        month = "0" + month;
    if (date < 10)
        date = "0" + date;
    return day.getFullYear() + "" + month + "" + date;
}

function formatDateDDMMYYYY(dateTime) {
    var day = new Date(dateTime);
    var month = day.getMonth();
    var date = day.getDate();
    if (month < 10)
        month = "0" + month;
    if (date < 10)
        date = "0" + date;
    return date + "/" + month + "/" + day.getFullYear();
}

function formatDateDDMMYYYYForEx(dateTime) {
    var day = new Date(dateTime);
    var month = parseInt(day.getMonth()) + 1;
    var date = day.getDate();
    if (month < 10)
        month = "0" + month;
    if (date < 10)
        date = "0" + date;
    return date + "/" + month + "/" + day.getFullYear();
}
function formatDateDDMMYYYYForExv2(dateTime) {
    var day = new Date(dateTime);
    var month = day.getMonth() + 1;
    var date = day.getDate();
    if (month < 10)
        month = "0" + (parseInt(month));
    if (date < 10)
        date = "0" + date;
    return date + "/" + month + "/" + day.getFullYear();
}

function formatDateDDMMYYYYHHMMForEx(dateTime) {
    var day = new Date(dateTime);
    var month = day.getMonth() + 1;
    var date = day.getDate();
    var hour = day.getHours();
    if (hour < 10)
        hour = "0" + (parseInt(hour));
    var minute = day.getMinutes();
    if (minute < 10)
        minute = "0" + (parseInt(minute));
    var second = day.getSeconds();
    if (second < 10)
        second = "0" + (parseInt(second));
    if (month < 10)
        month = "0" + (parseInt(month));
    if (date < 10)
        date = "0" + date;
    return date + "/" + month + "/" + day.getFullYear() + " " + hour + ":" + minute + ":" + second;
}

function formatDateMMDDYYYY(dateTime) {
    var day = new Date(dateTime);
    var month = day.getMonth() + 1;
    var date = day.getDate();
    if (month < 10)
        month = "0" + month;
    if (date < 10)
        date = "0" + date;
    return month + "/" + date + "/" + day.getFullYear();
}

function formatDateChar(dateTime, char) {
    var day = new Date(dateTime);
    var month = day.getMonth();
    var date = day.getDate();
    if (month < 10)
        month = "0" + month;
    if (date < 10)
        date = "0" + date;
    return day.getFullYear() + char + month + char + date;
}

function reFormatDate(dateNumber) {
    var result = new Date(dateNumber.substring(0, 4), dateNumber.substring(4, 6), dateNumber.substring(6, 8));
    return result;
}

function formatDateKendo(dateTime) {
    try {
        return kendo.toString(dateTime, "s");
    }
    catch (e) {
        console.log(">>formatDateKendo:", e, dateTime);
        return null;
    }
    return dateTime;
};
function setObjectValue(objTemplate, objValue) {
    var result = objTemplate;
    var script = "";
    for (var property in objTemplate) {
        script += " if(typeof(objValue." + property + ") != undefined) { result." + property + " = objValue." + property + "; }; ";
    }
    if (script.length > 0) {
        eval(script);
    }
    return result;
}

function setObjectValueV2(objTemplate, objValue) {
    for (var property in objTemplate) {
        var value = objValue[property];
        if (typeof (value) !== 'undefined') {
            objTemplate[property] = objValue[property];
        }
    }
    return objTemplate;
}

String.prototype.ReplaceAll = function (stringToFind, stringToReplace) {
    var temp = this;
    var index = temp.indexOf(stringToFind);
    while (index != -1) {
        temp = temp.replace(stringToFind, stringToReplace);
        index = temp.indexOf(stringToFind);
    }
    return temp;
};

String.prototype.SubstringByWord = function (numberOfWord, endValue) {
    if (endValue == null || typeof (endValue) == undefined) {
        endValue = "...";
    }
    var temp = this;
    //remove whitespace
    temp = temp.ReplaceAll("  ", " ");

    var words = temp.split(" ", numberOfWord);
    temp = "";
    for (var i = 0; i < words.length; i++) {
        if (temp.length > 0) {
            temp += " "
        }
        temp += words[i];
    }

    if (temp.length < this.length) {
        console.log(">>", temp.length, this.length);
        temp += endValue;
    }


    return temp;
};

function getBeginDateInMonth() {
    var date = new Date(), y = date.getFullYear(), m = date.getMonth();
    var firstDay = new Date(y, m, 1);
    return firstDay;
}
function getEndDateInMonth() {
    var date = new Date(), y = date.getFullYear(), m = date.getMonth();
    var lastDay = new Date(y, m + 1, 0);
    return lastDay;
}

function getFinanceBeginDate(year) {
    return new Date(year, 3, 1, 8, 0, 0, 0);
}
function getFinanceEndDate(year) {
    return new Date(year + 1, 2, 31, 8, 0, 0, 0);
}
function validatePhoneNumber(pn) {
    if (!isNaN(pn) && pn.length < 12) return true;
    return false;
}

function getFinanceYearByDate(date) {
    if (date.setHours(23, 59, 59) >= getFinanceBeginDate(date.getFullYear()) && date.setHours(0, 0, 0) <= getFinanceEndDate(date.getFullYear())) {
        return date.getFullYear();
    }
    else return date.getFullYear() - 1;
}
function checkFinanceYear(fromDate, toDate) {
    var year = getFinanceYearByDate(fromDate);
    //console.log("yearFinance >> ", year);
    if (fromDate.setHours(23, 59, 59) >= getFinanceBeginDate(year) && fromDate.setHours(0, 0, 0) <= getFinanceEndDate(year)
        && toDate.setHours(23, 59, 59) >= getFinanceBeginDate(year) && toDate.setHours(0, 0, 0) <= getFinanceEndDate(year)) {
        //console.log("fromDate: ", new Date(fromDate.setHours(23, 59, 59)));
        return true;
    }
    else return false;
}

// Get Date in ONE finance Year.
function checkSameDateFinanceYear(fromDate, toDate) {
    //var fromDate = new Date($scope.filter.FromDate);
    //var toDate = new Date($scope.filter.ToDate);
    var numDate = dateDiff(fromDate, toDate) + 1;
    if (numDate <= 0 || checkFinanceYear(fromDate, toDate) == false) {
        var notifyMessage = "Vui lòng chọn trong cùng năm tài chính!";
        notification(notifyMessage, "error");
        //$scope.filter.FromDate = getFinanceBeginDate($scope.FinanceYear);
        //$scope.filter.ToDate = getFinanceEndDate($scope.FinanceYear);
        return 0;
    }
    else return 1;
}


//function CheckSameFinanceYear(fromDate, toDate) {
//    console.log("fromDate", fromDate, " toDate", toDate);
//    //var fromDate = new Date($scope.filter.FromDate);
//    //var toDate = new Date($scope.filter.ToDate);
//    var numDate = dateDiff(fromDate, toDate) + 1;
//    if (numDate <= 0 || checkFinanceYear(fromDate, toDate) == false) {
//        var notifyMessage = "Vui lòng chọn đúng năm tài chính!";
//        notification(notifyMessage, "error");
//        //$scope.filter.FromDate = getFinanceBeginDate($scope.FinanceYear);
//        //$scope.filter.ToDate = getFinanceEndDate($scope.FinanceYear);
//        return 0;
//    }
//    else return 1;
//}
// Distint array
function onlyUnique(value, index, self) {
    return self.indexOf(value) === index;
}

// Loc dau tieng viet
function removeTiengViet(str) {
    str = str.toLowerCase();
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/!|@|\$|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\'| |\"|\&|\#|\[|\]|~/g, "-");
    str = str.replace(/-+-/g, "-"); //thay thế 2- thành 1-
    str = str.replace(/^\-+|\-+$/g, "");//cắt bỏ ký tự - ở đầu và cuối chuỗi
    return str;
}

// Loc dau tieng viet
function removeTiengVietVaKhoangTrang(str) {
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
    str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "A");
    str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
    str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
    str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
    str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
    str = str.replace(/Đ/g, "D");
    str = str.replace(/!|@|\$|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\'| |\"|\&|\#|\[|\]|~/g, "");
    str = str.replace(/-+-/g, ""); //thay thế 2- thành 1-
    str = str.replace(/^\-+|\-+$/g, "");//cắt bỏ ký tự - ở đầu và cuối chuỗi
    return str;
}
// Loại bỏ HTML khỏi chuỗi 
function strip_tags(str, allowed_tags) {
    var key = '', allowed = false;
    var matches = [];
    var allowed_array = [];
    var allowed_tag = '';
    var i = 0;
    var k = '';
    var html = '';

    var replacer = function (search, replace, str) {
        return str.split(search).join(replace);
    };
    // Build allowes tags associative array
    if (allowed_tags) {
        allowed_array = allowed_tags.match(/([a-zA-Z0-9]+)/gi);
    }

    str += '';

    // Match tags
    matches = str.match(/(<\/?[\S][^>]*>)/gi);
    if (matches != null) {
        matches.push("&nbsp;");
    }

    // Go through all HTML tags
    for (key in matches) {
        if (isNaN(key)) {
            // IE7 Hack
            continue;
        }

        // Save HTML tag
        html = matches[key].toString();

        // Is tag not in allowed list ? Remove from str !
        allowed = false;

        // Go through all allowed tags
        for (k in allowed_array) {
            // Init
            allowed_tag = allowed_array[k];
            i = -1;

            if (i != 0) {
                i = html.toLowerCase().indexOf('<' + allowed_tag + '>');
            }
            if (i != 0) {
                i = html.toLowerCase().indexOf('<' + allowed_tag + ' ');
            }
            if (i != 0) {
                i = html.toLowerCase().indexOf('</' + allowed_tag);
            }


            // Determine
            if (i == 0) {
                allowed = true;
                break;
            }
        }

        if (!allowed) {
            str = replacer(html, "", str);
            // Custom replace. No regexing
        }
    }

    return str;
}

function convertToDate(moment, input) {
    //console.log(">>ConvertToDate", input, typeof(input));
    if (input != null) {
        var value = input + Common.GMT;
        //console.log(">>>>ConvertToDatevalue", value, typeof (value));
        //var result = new Date(value);
        var result = moment(value).toDate();
        //console.log(">>>>ConvertToDateResult", result, typeof (result));
        return result;
    }
    return new Date();
}

function convertDate(input) {
    if (input != null) {
        var d = new Date(input);
        d.setHours(8);
        return d;
    }
    return new Date();
}

// -- convert string: "1,2,3,4" --> Object {"1", "2", "3", "4"}
function convertStringToObject(input) {
    //  console.log("input: ", input, typeof (input));
    if (input != null && input != "") {
        return eval(input.split(','));
    }
    return null;
}

function timeAgo(input) {
    if (input != null) {
        var value = input + Common.GMT;
        var result = moment(value).fromNow();
        return result;
    }
}

function validateSystemRule(documentProperties, conditions) {
    var result = new Array();
    var o = documentProperties;
    var valid = true;
    for (var index = 0; index < conditions.length && valid == true; index++) {
        var expression = conditions[index];
        //console.log(">>ValidateSystemRule", expression);
        valid = eval(expression);
        //console.log(">>ValidateSystemRule: result", valid);
    }
    return result;
}


function activeMenu(index, childLi, title) {
    //console.log("index>>>>>>> ", index, childLi, title)
    if (index == 2) {
        //    $('#mainnav-menu li ul').each(function () {
        //        if ($(this).hasClass('in')) {
        //            $(this).parent().removeClass('active');
        //            $(this).removeClass('in');
        //        }
        //    });
        $('#mainnav-menu li').removeClass("active-link");
        childLi.parent('li.lv2').addClass('active-link');
    }
    if (index > 2) {
        if (childLi.parent('li.lv' + index).parent().parent().hasClass('active-link active')) {
            childLi.parent('li.lv' + index).parent().children('li').removeClass('active-link');
        }
        childLi.parent('li.lv' + index).addClass('active-link');
    }
}

function reloadKendoDataSourceUrl(dataSource) {
    reloadKendoDataSourceUrl(dataSource, null);
}

function reloadKendoDataSourceUrl(dataSource, url) {
    if (typeof (url) != "undefined" && url != null && url.length > 0) {
        dataSource.options.transport.read = url;
        dataSource.transport.options.read.url = url;
        dataSource.transport.options.read.beforeSend = applyKendoHeader;
    }
    dataSource.page(1);
    dataSource.read();
    return dataSource;
}
// Loc danh sach ADN ( 00, 0001, 01 ) -> (00 , 01) 
// listADN is Object
function filterGroupADN(listADN) {
    if (!isNullOrEmpty(listADN)) {
        for (var i = 0; i < listADN.length; i++) {
            var checkChild = false;
            for (var j = i + 1; j < listADN.length; j++) {
                if (listADN[i].startsWith(listADN[j]))
                    checkChild = true;
                if (listADN[j].startsWith(listADN[i])) {
                    listADN.splice(j, 1);
                    j--;
                }
            }
            if (checkChild) {
                listADN.splice(i, 1);
                i--;
            }
        }
        console.log("listADN>>>> ", listADN);
        return listADN.toString();
    }
    else return "";
}


String.prototype.SubstringByWord = function (numberOfWord, endValue) {
    if (endValue == null || typeof (endValue) == undefined) {
        endValue = "...";
    }
    var temp = this;
    //remove whitespace
    temp = temp.ReplaceAll("  ", " ");

    var words = temp.split(" ", numberOfWord);
    temp = "";
    for (var i = 0; i < words.length; i++) {
        if (temp.length > 0) {
            temp += " "
        }
        temp += words[i];
    }

    if (temp.length < this.length) {
        temp += endValue;
    }

    return temp;
};

function checkIsLockFinance(isLockFinance, expiryDate) {
    var currentDate = new Date(formatDateKendo(new Date()));
    expiryDate = new Date(expiryDate);
    var numDate = getTimeByMinute(currentDate, expiryDate);
    if (isLockFinance == true && numDate < 0) {
        return true;
    }
    return false;
}

// Chia array thanh` theo các cột
function array_chuck(arrayInput, arraySize) {
    var temparray = new Array();
    for (var i = 0; i < arrayInput.length; i += arraySize) {
        temparray[temparray.length] = arrayInput.slice(i, i + arraySize);
        // do whatever
    }
    return temparray;
}

function notificationDesktop(title, body, icon, url) {
    var notify;

    if (Notification.permission === 'default') {
        console.log('request permission');
    }
    else {
        notify = new Notification(title, {
            body: body,
            icon: icon
        });

        notify.onclick = function (e) {
            openPage(url);
            notify.close();
        };

        setTimeout(function () {
            notify.close()
        }, 10000);
    }
}
function html2value(html) {
    return $("<div>").html(html).text();
}

Number.prototype.padLeft = function (base, chr) {
    var len = (String(base || 10).length - String(this).length) + 1;
    return len > 0 ? new Array(len).join(chr || '0') + this : this;
}

function popup_content(title, html) {
    $("#div_popup").html(html);
    var myWindow = $("#div_popup");
    myWindow.kendoWindow({
        width: "800px",
        title: title,
        visible: false,
        actions: [
            "Close"
        ]
    }).data("kendoWindow").center().open();
}

function convertDateddMMyyyy(inputFormat) {
    function pad(s) { return (s < 10) ? '0' + s : s; }
    var d = new Date(inputFormat);
    return [pad(d.getDate()), pad(d.getMonth() + 1), d.getFullYear()].join('/');
}
// kiểm tra fromDate <= toDate và chuẩn format 
function dateValid(fromDate, toDate) {
    if (!kendo.parseDate(fromDate) || !kendo.parseDate(toDate)) {
        var notifyMessage = MESSAGE_DATA_INVALID;
        notification(notifyMessage, "error");
        return false;
    }
    else {
        var numDate = dateDiff(fromDate, toDate) + 1;
        if (numDate <= 0) {
            var notifyMessage = MESSAGE_CHANGE_DATE_INVALID;
            notification(notifyMessage, "error");
            return false;
        }
    }
    return true;
}
////Kiểm tra fromDate, toDate cùng năm tài chính
//function checkFinanceYear(fromDate, toDate) {
//    var year = getFinanceYearByDate(fromDate);
//    //console.log("yearFinance >> ", year);
//    if (fromDate.setHours(23, 59, 59) >= getFinanceBeginDate(year) && fromDate.setHours(0, 0, 0) <= getFinanceEndDate(year)
//        && toDate.setHours(23, 59, 59) >= getFinanceBeginDate(year) && toDate.setHours(0, 0, 0) <= getFinanceEndDate(year)) {
//        //console.log("fromDate: ", new Date(fromDate.setHours(23, 59, 59)));
//        return true;
//    }
//    else {
//        var notifyMessage = MESSAGE_FINANCEYEAR_INVALID;
//        notification(notifyMessage, "error");
//        return false
//    };
//}

function checkDateInvalid(dateFrom, dateTo) {
    var now = new Date();
    var numDateFrom = dateDiff(dateFrom, now) + 1;
    var numDateTo = dateDiff(now, dateTo) + 1;
    if (numDateFrom > 0 && numDateTo > 0) {
        return true;
    }
    return false;
}

function checkIsDate(date) {
    return date instanceof Date;
}

function getTokenHeader() {
    return $("#CURRENT_USER_TOKEN").val();
}

function getEmailHeader() {
    return $("#CURRENT_USER_EMAIL").val();
}

function applyKendoHeader(req) {
    var tokenKey = $("#CURRENT_USER_TOKEN").val();
    req.setRequestHeader('Authorization', 'Bearer ' + tokenKey);
}

function applyKendoHeaderUpload(e) {
    var tokenKey = $("#CURRENT_USER_TOKEN").val();
    var xhr = e.XMLHttpRequest;
    if (xhr) {
        xhr.addEventListener("readystatechange", function (e) {
            if (xhr.readyState === 1 /* OPENED */) {
                xhr.setRequestHeader('Authorization', 'Bearer ' + tokenKey);
            }
        });
    }
}


function setKendoHeader(dataSource) {
    //console.log(">>setKendoHeader", dataSource);
    dataSource.options.transport.read.beforeSend = applyKendoHeader;
    //dataSource.transport.options.read.beforeSend = applyKendoHeader;
    return dataSource;
}

function compareNumber(number1, number2) {
    number1 = Math.round(number1);
    number2 = Math.round(number2);
    if (parseInt(number1) == parseInt(number2)) {
        return true;
    }
    return false;
}

function compareNumberN1(number1, number2) {
    number1 = parseFloat(number1).toFixed(1);
    number2 = parseFloat(number2).toFixed(1);
    if (number1 == number2) {
        return true;
    }
    return false;
}
function compareNumberN2(number1, number2) {
    number1 = parseFloat(number1).toFixed(2);
    number2 = parseFloat(number2).toFixed(2);
    if (number1 == number2) {
        return true;
    }
    return false;
}


//ham chuyen so thanh chu
var ChuSo = new Array(" không ", " một ", " hai ", " ba ", " bốn ", " năm ", " sáu ", " bảy ", " tám ", " chín ");
var Tien = new Array("", " nghìn", " triệu", " tỷ", " nghìn tỷ", " triệu tỷ");
//1. Hàm đọc số có ba chữ số;
function cocSo3ChuSo(baso) {
    var tram;
    var chuc;
    var donvi;
    var KetQua = "";
    tram = parseInt(baso / 100);
    chuc = parseInt((baso % 100) / 10);
    donvi = baso % 10;
    if (tram == 0 && chuc == 0 && donvi == 0) return "";
    if (tram != 0) {
        KetQua += ChuSo[tram] + " trăm ";
        if ((chuc == 0) && (donvi != 0)) KetQua += " linh ";
    }
    if ((chuc != 0) && (chuc != 1)) {
        KetQua += ChuSo[chuc] + " mươi";
        if ((chuc == 0) && (donvi != 0)) KetQua = KetQua + " linh ";
    }
    if (chuc == 1) KetQua += " mười ";
    switch (donvi) {
        case 1:
            if ((chuc != 0) && (chuc != 1)) {
                KetQua += " mốt ";
            }
            else {
                KetQua += ChuSo[donvi];
            }
            break;
        case 5:
            if (chuc == 0) {
                KetQua += ChuSo[donvi];
            }
            else {
                KetQua += " lăm ";
            }
            break;
        default:
            if (donvi != 0) {
                KetQua += ChuSo[donvi];
            }
            break;
    }
    return KetQua;
}
//2. Hàm đọc số thành chữ (Sử dụng hàm đọc số có ba chữ số)
function cocTienBangChu(SoTien) {
    var lan = 0;
    var i = 0;
    var so = 0;
    var KetQua = "";
    var tmp = "";
    var ViTri = new Array();
    if (SoTien < 0) return "Số tiền âm !";
    if (SoTien == 0) return " Không ";
    if (SoTien > 0) {
        so = SoTien;
    }
    else {
        so = -SoTien;
    }
    if (SoTien > 8999999999999999) {
        //SoTien = 0;
        return "Số quá lớn!";
    }
    ViTri[5] = Math.floor(so / 1000000000000000);
    if (isNaN(ViTri[5]))
        ViTri[5] = "0";
    so = so - parseFloat(ViTri[5].toString()) * 1000000000000000;
    ViTri[4] = Math.floor(so / 1000000000000);
    if (isNaN(ViTri[4]))
        ViTri[4] = "0";
    so = so - parseFloat(ViTri[4].toString()) * 1000000000000;
    ViTri[3] = Math.floor(so / 1000000000);
    if (isNaN(ViTri[3]))
        ViTri[3] = "0";
    so = so - parseFloat(ViTri[3].toString()) * 1000000000;
    ViTri[2] = parseInt(so / 1000000);
    if (isNaN(ViTri[2]))
        ViTri[2] = "0";
    ViTri[1] = parseInt((so % 1000000) / 1000);
    if (isNaN(ViTri[1]))
        ViTri[1] = "0";
    ViTri[0] = parseInt(so % 1000);
    if (isNaN(ViTri[0]))
        ViTri[0] = "0";
    if (ViTri[5] > 0) {
        lan = 5;
    }
    else if (ViTri[4] > 0) {
        lan = 4;
    }
    else if (ViTri[3] > 0) {
        lan = 3;
    }
    else if (ViTri[2] > 0) {
        lan = 2;
    }
    else if (ViTri[1] > 0) {
        lan = 1;
    }
    else {
        lan = 0;
    }
    for (i = lan; i >= 0; i--) {
        tmp = DocSo3ChuSo(ViTri[i]);
        KetQua += tmp;
        if (ViTri[i] > 0) KetQua += Tien[i];
        if ((i > 0) && (tmp.length > 0)) KetQua += ',';//&& (!string.IsNullOrEmpty(tmp))
    }
    if (KetQua.substring(KetQua.length - 1) == ',') {
        KetQua = KetQua.substring(0, KetQua.length - 1);
    }
    KetQua = KetQua.substring(1, 2).toUpperCase() + KetQua.substring(2);
    return KetQua;//.substring(0, 1);//.toUpperCase();// + KetQua.substring(1);
}

function setClientStorageType(key, value) {
    var valueEncode = window.TextDecoder ? new TextEncoder().encode(value) : value;
    if (CLIENT_STORAGE_TYPE == 1) {
        sessionStorage.setItem(key, valueEncode);
    }
    else {
        localStorage.setItem(key, valueEncode);
    }
}

function getTextDecoder() {
    try {
        if (window.TextDecoder) {
            return new TextDecoder('utf-8');
        }
    }
    catch (e) {

    }
    return null;
}
function getUint8Array(value) {
    try {
        if (window.Uint8Array) {
            return new Uint8Array(value.split(","));
        }
    }
    catch (e) {

    }
    return value;
}

function getClientStorageType(key) {
    var value = (CLIENT_STORAGE_TYPE == 1) ? sessionStorage.getItem(key) : localStorage.getItem(key);
    if (isNullOrEmpty(value) == false) {
        var enc = getTextDecoder();
        var arr = getUint8Array(value);
        if (enc != null) {
            return enc.decode(arr);
        }
        return value;
    }
}

function removeClientStorageType(key) {
    if (CLIENT_STORAGE_TYPE == 1) {
        return sessionStorage.removeItem(key);
    }
    else {
        return localStorage.removeItem(key);
    }
}

function b64DecodeUnicode(str) {
    // Going backwards: from bytestream, to percent-encoding, to original string.
    return decodeURIComponent(atob(str).split('').map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));
}

function jsonToCSVConvertor(JSONData, ReportTitle, ShowLabel) {
    //If JSONData is not an object then JSON.parse will parse the JSON string in an Object
    var arrData = typeof JSONData != 'object' ? JSON.parse(JSONData) : JSONData;

    var CSV = '';
    //Set Report title in first row or line

    //CSV += ReportTitle + '\r\n\n';

    //This condition will generate the Label/Header
    if (ShowLabel) {
        var row = "";

        //This loop will extract the label from 1st index of on array
        for (var index in arrData[0]) {

            //Now convert each value to string and comma-seprated
            row += index + ',';
        }

        row = row.slice(0, -1);

        //append Label row with line break
        CSV += row + '\r\n';
    }

    //1st loop is to extract each row
    for (var i = 0; i < arrData.length; i++) {
        var row = "";

        //2nd loop will extract each column and convert it in string comma-seprated
        for (var index in arrData[i]) {
            row += '"' + arrData[i][index] + '",';
        }

        row.slice(0, row.length - 1);

        //add a line break after each row
        CSV += row + '\r\n';
    }

    if (CSV == '') {
        alert("Invalid data");
        return;
    }

    //Generate a file name
    var fileName = "";
    //this will remove the blank-spaces from the title and replace it with an underscore
    fileName += ReportTitle.replace(/ /g, "_");

    //Initialize file format you want csv or xls
    var uri = 'data:text/csv;charset=utf-8,' + escape(CSV);

    // Now the little tricky part.
    // you can use either>> window.open(uri);
    // but this will not work in some browsers
    // or you will not get the correct file extension    

    //this trick will generate a temp <a /> tag
    var link = document.createElement("a");
    link.href = uri;

    //set the visibility hidden so it will not effect on your web-layout
    link.style = "visibility:hidden";
    link.download = fileName + ".csv";

    //this part will append the anchor tag and remove it after automatic click
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}



function getFeatureIDByUrl(url) {
    var result = [];
    for (var i = 0; i < FeatureJSON.length; i++) {
        var feature = FeatureJSON[i];
        //console.log(feature.ModuleUrl);
        if (feature.ModuleUrl.indexOf(url) != -1 || url.indexOf(feature.ModuleUrl) != -1) {
            var a = (feature.ModuleUrl.indexOf(url) != -1) ? url.length : 0;
            var b = url.indexOf(feature.ModuleUrl) != -1 ? feature.ModuleUrl.length : 0;
            var length = Math.max(a, b);
            result[result.length] = { ID: feature.ID, Length: length };
        }
    }
    if (result.length > 0) {
        var maxLenth = -1;
        var index = -1;
        for (var i = 0; i < result.length; i++) {
            var obj = result[i];

            if (obj.Length > maxLenth) {
                maxLenth = obj.Length;
                index = obj.ID;
            }
        }
        return index;
    }
    return 0;
}

function openPopup(id) {
    $("#" + id).modal({ backdrop: 'static', keyboard: false });
}
function closePopup(id) {
    $("#" + id).modal("hide");
}
function convertDocumentType(documentType) {
    documentType = parseInt(documentType);
    //console.log("documemt: ", documentType);
    var Name = "";

    switch (documentType) {
        case 1:
            {
                Name = "";
                break;
            }
        case 2:
            {
                Name = "";
                break;
            }
        case 3:
            {
                Name = "";
                break;
            }
        case 4:
            {
                Name = "";
                break;
            }
        case 7:
            {
                Name = "";
                break;
            }
        case 8:
            {
                Name = "Hợp đồng";
                break;
            }
        case 9:
            {
                Name = "Phương án kinh doanh";
                break;
            }
        case 10:
            {
                Name = "";
                break;
            }
        case 11:
            {
                Name = "";
                break;
            }
        case 12:
            {
                Name = "";
                break;
            }
        case 21:
            {
                Name = "Cơ hội kinh doanh";
                break;
            }
        case 24:
            {
                Name = "";
                break;
            }
        case 26:
            {
                Name = "";
                break;
            }
        case 27:
            {
                Name = "";
                break;
            }
        case 28:
            {
                Name = "";
                break;
            }
        case 29:
            {
                Name = "";
                break;
            }
        case 30:
            {
                Name = "";
                break;
            }
        case 33:
            {
                Name = "";
                break;
            }
        case 34:
            {
                Name = "";
                break;
            }
        case 36:
            {
                Name = "";
                break;
            }
        case 37:
            {
                Name = "";
                break;
            }
        case 38:
            {
                Name = "";
                break;
            }
        case 39:
            {
                Name = "";
                break;
            }
        case 40:
            {
                Name = "";
                break;
            }
        case 41:
            {
                Name = "Đề xuất công cụ dụng cụ";
                break;
            }
        case 51:
            {
                Name = "";
                break;
            }
        case 54:
            {
                Name = "";
                break;
            }
        case 58:
            {
                Name = "";
                break;
            }
        case 59:
            {
                Name = "Yêu cầu mua hàng";
                break;
            }
        case 60:
            {
                Name = "";
                break;
            }
        case 62:
            {
                Name = "Báo giá";
                break;
            }
        case 64:
            {
                Name = "";
                break;
            }
        case 67:
            {
                Name = "";
                break;
            }
        case 68:
            {
                Name = "";
                break;
            }
        case 69:
            {
                Name = "";
                break;
            }
        case 71:
            {
                Name = "";
                break;
            }
        case 73:
            {
                Name = "";
                break;
            }
        case 74:
            {
                Name = "";
                break;
            }
        case 75:
            {
                Name = "";
                break;
            }
        case 77:
            {
                Name = "";
                break;
            }
        case 78:
            {
                Name = "";
                break;
            }
        case 83:
            {
                Name = "";
                break;
            }
        case 84:
            {
                Name = "";
                break;
            }
        case 85:
            {
                Name = "";
                break;
            }
        case 87:
            {
                Name = "";
                break;
            }
        case 90:
            {
                Name = "";
                break;
            }
        case 93:
            {
                Name = "";
                break;
            }
        case 97:
            {
                Name = "";
                break;
            }
        case 104:
            {
                Name = "Thẩm định tín dụng khách hàng";
                break;
            }
        case 106:
            {
                Name = "Kế hoạch tiêu thụ";
                break;
            }
        case 107:
            {
                Name = "Kế hoạch tiêu thụ tổng hợp";
                break;
            }
        case 108:
            {
                Name = "Chính sách giá phân phối";
                break;
            }
        case 110:
            {
                Name = "Đơn hàng";
                break;
            }
        case 113:
            {
                Name = "Phương án mua hàng";
                break;
            }
        case 119:
            {
                Name = "Đăng ký làm ngoài giờ";
                break;
            }
        case 121:
            {
                Name = "Quyết toán dự án";
                break;
            }
        case 114:
            {
                Name = "Kế hoạch mua hàng ủy quyền ASP";
                break;
            }
        case 115:
            {
                Name = "Yêu cầu sản xuất";
                break;
            }
        case 124:
            {
                Name = "Đánh giá thăm dò";
                break;
            }
        case 125:
            {
                Name = "Quyết toán phân phối";
                break;
            }
        case 126:
            {
                Name = "Quyết toán dịch vụ";
                break;
            }
    }
    return Name;
}

var branchesCommon = {
    TVK: 1,
    KhoChinh: 2
}

var BuildVersion = document.getElementById("BuildVersion").value;