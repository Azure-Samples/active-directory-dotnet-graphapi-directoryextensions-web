//NameControl.js file borrowed from http://answer solution.
//This file is also available as part of code sample at this url 
//http://www.microsoft.com/en-us/download/details.aspx?id=10003

var PresenceControlbrowseris = new PresenceControlBrowseris();
var PresenceControlObject = null;
var bPresenceControlInited = false;

var PresenceControlURIDictionaryObj = null;
var PresenceControlStatesDictionaryObj = null;

var PresenceControlOrigScrollFunc = null;
var bPresenceControlInScrollFunc = false;


function EnsurePresenceControl() {
    if (!bPresenceControlInited) {
        if (PresenceControlbrowseris.ie5up && PresenceControlbrowseris.win32) {
            PresenceControlObject = new ActiveXObject("Name.NameCtrl.1");
        }
        bPresenceControlInited = true;
        if (PresenceControlObject) {
            PresenceControlObject.OnStatusChange = PresenceControlOnStatusChange;
        }
    }
    return PresenceControlObject;
}


function PresenceControlOnStatusChange(name, state, id) {
    if (PresenceControlStatesDictionaryObj) {
        var img = PresenceControlGetStatusImage(state);
        if (PresenceControlStatesDictionaryObj[id] != state) {
            PresenceControlUpdateImage(id, img);
            PresenceControlStatesDictionaryObj[id] = state;
        }
    }
}

function PresenceControlShowOOUIMouse() {
    PresenceControlShowOOUI(0);
}

function PresenceControlShowOOUIFocus() {
    PresenceControlShowOOUI(1);
}

function PresenceControlShowOOUI(inputType) {
    if (PresenceControlbrowseris.ie5up && PresenceControlbrowseris.win32) {
        var obj = window.event.srcElement;
        var objSpan = obj;
        var objOOUI = obj;
        var oouiX = 0, oouiY = 0;
        if (EnsurePresenceControl() && PresenceControlURIDictionaryObj) {
            var objRet = PresenceControlGetOOUILocation(obj, false);

            objSpan = objRet.objSpan;
            objOOUI = objRet.objOOUI;
            oouiX = objRet.oouiX;
            oouiY = objRet.oouiY;
            var name = PresenceControlURIDictionaryObj[objOOUI.id];
            if (objSpan)
                objSpan.onkeydown = PresenceControlHandleAccelerator;
            PresenceControlObject.ShowOOUI(name, inputType, oouiX, oouiY - $(window).scrollTop());
        }
    }
}


function PresenceControlHideOOUI() {
    PresenceControlObject.HideOOUI();
}

function PresenceControlGetStatusImage(state) {
   // alert(state);
    var img = "presence_16-unknown.png";    
    switch (state) {
        case 0:
            img = "presence_16-online.png";
            break;
        case 1:
            img = "presence_16-off.png";
            break;
        case 2:
            img = "presence_16-away.png";
            break;
        case 3:
            img = "presence_16-busy.png";
            break;
        case 4:
            img = "presence_16-away.png";
            break;
        case 5:
            img = "presence_16-away.png";
            break;
        case 6:
            img = "presence_16-away.png";
            break;
        case 8:
            img = "presence_16-away.png";
            break;
        case 9:
            img = "presence_16-dnd.png";
            break;
        case 16:
            //img = "presence_16-idle-online.png";
            img = "presence_16-away.png";
            break;

    }
    return img;
}

function PresenceControlUpdateImage(id, img) {
    var obj = document.images(id);
    //alert('PresenceControlUpdateImage ' + obj);
    if (obj) {
        var oldImg = obj.src;
        var index = oldImg.lastIndexOf("/");
        var newImg = oldImg.slice(0, index + 1);
        newImg += img;
        if (oldImg != newImg)
            obj.src = newImg;
        if (obj.altbase) {
            obj.alt = obj.altbase;
        }
    }
}

function PresenceControlScroll() {
    if (!bPresenceControlInScrollFunc) {
        bPresenceControlInScrollFunc = true;
        PresenceControlHideOOUI();
    }
    bPresenceControlInScrollFunc = false;
    return PresenceControlOrigScrollFunc ? PresenceControlOrigScrollFunc() : true;
}


function PresenceControl(uri) {
    //alert(uri);
    //try {
        if (uri == null || uri == '')
            return;

        if (PresenceControlbrowseris.ie5up && PresenceControlbrowseris.win32) {

            var obj = window.event.srcElement;
            var objSpan = obj;
            var id = obj.id;
            var fFirst = false;
            if (!PresenceControlStatesDictionaryObj) {
                PresenceControlStatesDictionaryObj = new Object();
                PresenceControlURIDictionaryObj = new Object();
                if (!PresenceControlOrigScrollFunc) {
                    PresenceControlOrigScrollFunc = window.onscroll;
                    window.onscroll = PresenceControlScroll;
                    //alert('objSpan ' + objSpan);
                }
            }
            if (PresenceControlStatesDictionaryObj) {
                if (!PresenceControlURIDictionaryObj[id]) {
                    PresenceControlURIDictionaryObj[id] = uri;
                    fFirst = true;
                    //alert('fFirst ' + fFirst);
                }
                if (typeof (PresenceControlStatesDictionaryObj[id]) == "undefined") {
                    PresenceControlStatesDictionaryObj[id] = 1;
                }
                if (fFirst && EnsurePresenceControl() && PresenceControlObject.PresenceEnabled) {
                    var state = 1, img;
                    state = PresenceControlObject.GetStatus(uri, id);
                    //alert('state ' + fFirst);
                    img = PresenceControlGetStatusImage(state);
                    PresenceControlUpdateImage(id, img);
                    PresenceControlStatesDictionaryObj[id] = state;
                }

            }
            if (fFirst) {
                var objRet = PresenceControlGetOOUILocation(obj, false);
                objSpan = objRet.objSpan;
                if (objSpan) {
                    objSpan.onmouseover = PresenceControlShowOOUIMouse;
                    objSpan.onfocusin = PresenceControlShowOOUIFocus;
                    objSpan.onmouseout = PresenceControlHideOOUI;
                    objSpan.onfocusout = PresenceControlHideOOUI;
                }
            }
        }
//    }
//    catch (e) {
//        alert(e);
//    }
}

function PresenceControlWithoutImage(uri) {
    //alert(uri);
    //try {
    if (uri == null || uri == '')
        return;

    if (PresenceControlbrowseris.ie5up && PresenceControlbrowseris.win32) {

        var obj = window.event.srcElement;
        var objSpan = obj;
        var id = obj.id;
        var fFirst = false;
        if (!PresenceControlStatesDictionaryObj) {
            PresenceControlStatesDictionaryObj = new Object();
            PresenceControlURIDictionaryObj = new Object();
            if (!PresenceControlOrigScrollFunc) {
                PresenceControlOrigScrollFunc = window.onscroll;
                window.onscroll = PresenceControlScroll;
                //alert('objSpan ' + objSpan);
            }
        }
        if (PresenceControlStatesDictionaryObj) {
            if (!PresenceControlURIDictionaryObj[id]) {
                PresenceControlURIDictionaryObj[id] = uri;
                fFirst = true;
                //alert('fFirst ' + fFirst);
            }
            if (typeof (PresenceControlStatesDictionaryObj[id]) == "undefined") {
                PresenceControlStatesDictionaryObj[id] = 1;
            }
            if (fFirst && EnsurePresenceControl() && PresenceControlObject.PresenceEnabled) {
//                var state = 1, img;
//                state = PresenceControlObject.GetStatus(uri, id);
//                //alert('state ' + fFirst);
//                img = PresenceControlGetStatusImage(state);
//                PresenceControlUpdateImage(id, img);
//                PresenceControlStatesDictionaryObj[id] = state;
            }

        }
        if (fFirst) {
            var objRet = PresenceControlGetOOUILocation(obj, false);
            objSpan = objRet.objSpan;
            if (objSpan) {
                objSpan.onmouseover = PresenceControlShowOOUIMouse;
                objSpan.onfocusin = PresenceControlShowOOUIFocus;
                objSpan.onmouseout = PresenceControlHideOOUI;
                objSpan.onfocusout = PresenceControlHideOOUI;
            }
        }
    }
    //    }
    //    catch (e) {
    //        alert(e);
    //    }
}


function PresenceControlHandleAccelerator() {
    if (PresenceControlObject) {
        if (event.altKey && event.shiftKey &&
            event.keyCode == 121) {
            PresenceControlObject.DoAccelerator();
        }
    }
}

function PresenceControlGetOOUILocation(obj, fprint) {
    var objRet = new Object;
    var objSpan = obj;
    var objOOUI = obj;
    var oouiX = 0, oouiY = 0, objDX = 0;
    var fRtl = document.dir == "rtl";
    while (objSpan && objSpan.tagName != "SPAN" && objSpan.tagName != "TABLE") {
        objSpan = objSpan.parentNode;
    }
    if (objSpan) {
        var collNodes = objSpan.tagName == "TABLE" ?
                       objSpan.rows(0).cells(0).childNodes :
                       objSpan.childNodes;
        var i;
        for (i = 0; i < collNodes.length; ++i) {
            if (collNodes.item(i).tagName == "IMG" && collNodes.item(i).id) {
                objOOUI = collNodes.item(i);
                break;
            }
        }
    }
    obj = objOOUI;
    while (obj) {
        if (fRtl) {
            if (obj.scrollWidth >= obj.clientWidth + obj.scrollLeft)
                objDX = obj.scrollWidth - obj.clientWidth - obj.scrollLeft;
            else
                objDX = obj.clientWidth + obj.scrollLeft - obj.scrollWidth;
            oouiX += obj.offsetLeft + objDX;
        }
        else
            oouiX += obj.offsetLeft - obj.scrollLeft;
        oouiY += obj.offsetTop - obj.scrollTop;
        if (fprint) {
            alert(obj.scrollTop);
        }

        obj = obj.offsetParent;
    }
    try {
        obj = window.frameElement;
        while (obj) {
            if (fRtl) {
                if (obj.scrollWidth >= obj.clientWidth + obj.scrollLeft)
                    objDX = obj.scrollWidth - obj.clientWidth - obj.scrollLeft;
                else
                    objDX = obj.clientWidth + obj.scrollLeft - obj.scrollWidth;
                oouiX += obj.offsetLeft + objDX;
            }
            else
                oouiX += obj.offsetLeft - obj.scrollLeft;
            oouiY += obj.offsetTop - obj.scrollTop;
            if (fprint) {
                alert(obj.scrollTop);
            }

            obj = obj.offsetParent;
        }
    } catch (e) {
    };

    objRet.objSpan = objSpan;
    objRet.objOOUI = objOOUI;
    objRet.oouiX = oouiX;
    objRet.oouiY = oouiY;
    if (fRtl)
        objRet.oouiX += objOOUI.offsetWidth;

    if (fprint) {
        alert(oouiY);
    }

    return objRet;
}


function PresenceControlBrowseris() {
    var agt = navigator.userAgent.toLowerCase();
    this.osver = 1.0;
    if (agt) {
        var stOSVer = agt.substring(agt.indexOf("windows ") + 11);
        this.osver = parseFloat(stOSVer);
    }

    this.major = parseInt(navigator.appVersion);
    this.nav = ((agt.indexOf('mozilla') != -1) && ((agt.indexOf('spoofer') == -1) && (agt.indexOf('compatible') == -1)));
    this.nav2 = (this.nav && (this.major == 2));
    this.nav3 = (this.nav && (this.major == 3));
    this.nav4 = (this.nav && (this.major == 4));
    this.nav6 = this.nav && (this.major == 5);
    this.nav6up = this.nav && (this.major >= 5);
    this.nav7up = false;
    if (this.nav6up) {
        var navIdx = agt.indexOf("netscape/");
        if (navIdx >= 0)
            this.nav7up = parseInt(agt.substring(navIdx + 9)) >= 7;
    }
    this.ie = (agt.indexOf("msie") != -1);
    this.aol = this.ie && agt.indexOf(" aol ") != -1;
    if (this.ie) {
        var stIEVer = agt.substring(agt.indexOf("msie ") + 5);
        this.iever = parseInt(stIEVer);
        this.verIEFull = parseFloat(stIEVer);
    }
    else
        this.iever = 0;
    this.ie3 = (this.ie && (this.major == 2));
    this.ie4 = (this.ie && (this.major == 4));
    this.ie4up = this.ie && (this.major >= 4);
    this.ie5up = this.ie && (this.iever >= 5);
    this.ie55up = this.ie && (this.verIEFull >= 5.5);
    this.ie6up = this.ie && (this.iever >= 6);
    this.win16 = ((agt.indexOf("win16") != -1)
               || (agt.indexOf("16bit") != -1) || (agt.indexOf("windows 3.1") != -1)
               || (agt.indexOf("windows 16-bit") != -1));
    this.win31 = (agt.indexOf("windows 3.1") != -1) || (agt.indexOf("win16") != -1) ||
                 (agt.indexOf("windows 16-bit") != -1);
    this.win98 = ((agt.indexOf("win98") != -1) || (agt.indexOf("windows 98") != -1));
    this.win95 = ((agt.indexOf("win95") != -1) || (agt.indexOf("windows 95") != -1));
    this.winnt = ((agt.indexOf("winnt") != -1) || (agt.indexOf("windows nt") != -1));
    this.win32 = this.win95 || this.winnt || this.win98 ||
                 ((this.major >= 4) && (navigator.platform == "Win32")) ||
                 (agt.indexOf("win32") != -1) || (agt.indexOf("32bit") != -1);
    this.os2 = (agt.indexOf("os/2") != -1)
                 || (navigator.appVersion.indexOf("OS/2") != -1)
                 || (agt.indexOf("ibm-webexplorer") != -1);
    this.mac = (agt.indexOf("mac") != -1);
    this.mac68k = this.mac && ((agt.indexOf("68k") != -1) ||
                               (agt.indexOf("68000") != -1));
    this.macppc = this.mac && ((agt.indexOf("ppc") != -1) ||
                               (agt.indexOf("powerpc") != -1));
    this.w3c = this.nav6up;
}

// SIG // Begin signature block
// SIG // MIIaKgYJKoZIhvcNAQcCoIIaGzCCGhcCAQExCzAJBgUr
// SIG // DgMCGgUAMGcGCisGAQQBgjcCAQSgWTBXMDIGCisGAQQB
// SIG // gjcCAR4wJAIBAQQQEODJBs441BGiowAQS9NQkAIBAAIB
// SIG // AAIBAAIBAAIBADAhMAkGBSsOAwIaBQAEFP9Pa/IbB4wX
// SIG // y/rWpP+MsDMOXm4ZoIIUvDCCArwwggIlAhBKGdI4jIJZ
// SIG // HKVdc18VXdyjMA0GCSqGSIb3DQEBBAUAMIGeMR8wHQYD
// SIG // VQQKExZWZXJpU2lnbiBUcnVzdCBOZXR3b3JrMRcwFQYD
// SIG // VQQLEw5WZXJpU2lnbiwgSW5jLjEsMCoGA1UECxMjVmVy
// SIG // aVNpZ24gVGltZSBTdGFtcGluZyBTZXJ2aWNlIFJvb3Qx
// SIG // NDAyBgNVBAsTK05PIExJQUJJTElUWSBBQ0NFUFRFRCwg
// SIG // KGMpOTcgVmVyaVNpZ24sIEluYy4wHhcNOTcwNTEyMDAw
// SIG // MDAwWhcNMDQwMTA3MjM1OTU5WjCBnjEfMB0GA1UEChMW
// SIG // VmVyaVNpZ24gVHJ1c3QgTmV0d29yazEXMBUGA1UECxMO
// SIG // VmVyaVNpZ24sIEluYy4xLDAqBgNVBAsTI1ZlcmlTaWdu
// SIG // IFRpbWUgU3RhbXBpbmcgU2VydmljZSBSb290MTQwMgYD
// SIG // VQQLEytOTyBMSUFCSUxJVFkgQUNDRVBURUQsIChjKTk3
// SIG // IFZlcmlTaWduLCBJbmMuMIGfMA0GCSqGSIb3DQEBAQUA
// SIG // A4GNADCBiQKBgQDTLiDwaHwsLS6BHLEGsqcLtxENV9pT
// SIG // 2HXjyTMqstT2CVs08+mQ/gkM0NsbWrnN5/aIsZ3AhyXr
// SIG // fVgQc2p4y3EV/cZY9imrWF6WBP0tYhFYgRzKcZTVIlgv
// SIG // 1cwUBYQ2upSqtE1K6e47Iq1WmX4hnGyGwEpHl2q0pjbV
// SIG // /Akt07Q5mwIDAQABMA0GCSqGSIb3DQEBBAUAA4GBAGFV
// SIG // Dj57x5ISfhEQjiLM1LMTK1voROQLeJ6kfvOnB3Ie4lnv
// SIG // zITjiZRM205h77Ok+0Y9UDQLn3BW9o4qfxfO5WO/eWkH
// SIG // cy6wlSiK9e2qqdJdzQrKEAmPzrOvKJbEeSmEktz/umdC
// SIG // SKaQEOS/YficU+WT0XM/+P2dT4SsVdH9EWNjMIIEAjCC
// SIG // A2ugAwIBAgIQCHptXG9ik0+6xP1D4RQYnTANBgkqhkiG
// SIG // 9w0BAQQFADCBnjEfMB0GA1UEChMWVmVyaVNpZ24gVHJ1
// SIG // c3QgTmV0d29yazEXMBUGA1UECxMOVmVyaVNpZ24sIElu
// SIG // Yy4xLDAqBgNVBAsTI1ZlcmlTaWduIFRpbWUgU3RhbXBp
// SIG // bmcgU2VydmljZSBSb290MTQwMgYDVQQLEytOTyBMSUFC
// SIG // SUxJVFkgQUNDRVBURUQsIChjKTk3IFZlcmlTaWduLCBJ
// SIG // bmMuMB4XDTAxMDIyODAwMDAwMFoXDTA0MDEwNjIzNTk1
// SIG // OVowgaAxFzAVBgNVBAoTDlZlcmlTaWduLCBJbmMuMR8w
// SIG // HQYDVQQLExZWZXJpU2lnbiBUcnVzdCBOZXR3b3JrMTsw
// SIG // OQYDVQQLEzJUZXJtcyBvZiB1c2UgYXQgaHR0cHM6Ly93
// SIG // d3cudmVyaXNpZ24uY29tL3JwYSAoYykwMTEnMCUGA1UE
// SIG // AxMeVmVyaVNpZ24gVGltZSBTdGFtcGluZyBTZXJ2aWNl
// SIG // MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA
// SIG // wHphh+uypwNjGysaYd6AtxUdoIuQPbsnkoQUOeuFzimS
// SIG // BmZIpANPjehPp/CvXtEvGceR8bWee5Ehzun/407w/K+V
// SIG // WLhjLeaO9ikYzXCOUMPtlrtA274l6EJV1vaF8gbni5kc
// SIG // MfMDD9RMnCQq3Bsbj4LzsO+nTeMUp+CP1sdowmFYqXLU
// SIG // +DBIT9kvb2Mg2YnKgnvCS7woxYFo5+aCQKxGOqD5PzbN
// SIG // TLtUQlp6ZXv+hOTHR1SsuT3sgMca98QzgYHJKpX7f146
// SIG // h5AU28wudfLva+Y9qWC+QgGqT6pbqD8iMZ8SFflzoR6C
// SIG // iwQr6kYCTG2PH1AulUsqeAaEdD2RjyxHMQIDAQABo4G4
// SIG // MIG1MEAGCCsGAQUFBwEBBDQwMjAwBggrBgEFBQcwAYYk
// SIG // aHR0cDovL29jc3AudmVyaXNpZ24uY29tL29jc3Avc3Rh
// SIG // dHVzMAkGA1UdEwQCMAAwRAYDVR0gBD0wOzA5BgtghkgB
// SIG // hvhFAQcBATAqMCgGCCsGAQUFBwIBFhxodHRwczovL3d3
// SIG // dy52ZXJpc2lnbi5jb20vcnBhMBMGA1UdJQQMMAoGCCsG
// SIG // AQUFBwMIMAsGA1UdDwQEAwIGwDANBgkqhkiG9w0BAQQF
// SIG // AAOBgQAt809jYCwY2vUkD1KzDOuzvGeFwiPtj0YNzxpN
// SIG // vvN8eiAwMhhoi5K7Mpnwk7g7FQYnez4CBgCkIZKEEwrF
// SIG // mOVAV8UFJeivrxFqqeU7y+kj9pQpXUBV86VTncg2Ojll
// SIG // CHNzpDLSr6y/xwU8/0Xsw+jaJNHOY64Jp/viG+P9QQpq
// SIG // ljCCBBIwggL6oAMCAQICDwDBAIs8PIgR0T72Y+zfQDAN
// SIG // BgkqhkiG9w0BAQQFADBwMSswKQYDVQQLEyJDb3B5cmln
// SIG // aHQgKGMpIDE5OTcgTWljcm9zb2Z0IENvcnAuMR4wHAYD
// SIG // VQQLExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xITAfBgNV
// SIG // BAMTGE1pY3Jvc29mdCBSb290IEF1dGhvcml0eTAeFw05
// SIG // NzAxMTAwNzAwMDBaFw0yMDEyMzEwNzAwMDBaMHAxKzAp
// SIG // BgNVBAsTIkNvcHlyaWdodCAoYykgMTk5NyBNaWNyb3Nv
// SIG // ZnQgQ29ycC4xHjAcBgNVBAsTFU1pY3Jvc29mdCBDb3Jw
// SIG // b3JhdGlvbjEhMB8GA1UEAxMYTWljcm9zb2Z0IFJvb3Qg
// SIG // QXV0aG9yaXR5MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8A
// SIG // MIIBCgKCAQEAqQK9wXDmO/JOGyifl3heMOqiqY0lX/j+
// SIG // lUyjt/6doiA+fFGim6KPYDJr0UJkee6sdslU2vLrnIYc
// SIG // j5+EZrPFa3piI9YdPN4PAZLolsS/LWaammgmmdA6LL8M
// SIG // tVgmwUbnCj44liypKDmo7EmDQuOED7uabFVhrIJ8oWAt
// SIG // d0zpmbRkO5pQHDEIJBSfqeeRKxjmPZhjFGBYBWWfHTdS
// SIG // h/en75QCxhvTv1VFs4mAvzrsVJROrv2nem10Tq8YzJYJ
// SIG // KCEAV5BgaTe7SxIHPFb/W/ukZgoIptKBVlfvtjteFoF3
// SIG // BNr2vq6Alf6wzX/WpxpyXDzKvPAIoyIwswaFybMgdxOF
// SIG // 3wIDAQABo4GoMIGlMIGiBgNVHQEEgZowgZeAEFvQcO9p
// SIG // cp4jUX4Usk2O/8uhcjBwMSswKQYDVQQLEyJDb3B5cmln
// SIG // aHQgKGMpIDE5OTcgTWljcm9zb2Z0IENvcnAuMR4wHAYD
// SIG // VQQLExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xITAfBgNV
// SIG // BAMTGE1pY3Jvc29mdCBSb290IEF1dGhvcml0eYIPAMEA
// SIG // izw8iBHRPvZj7N9AMA0GCSqGSIb3DQEBBAUAA4IBAQCV
// SIG // 6AvAjfOXGDXtuAEk2HcR81xgMp+eC8s+BZGIj8k65iHy
// SIG // 8FeTLLWgR8hi7/zXzDs7Wqk2VGn+JG0/ycyq3gV83TGN
// SIG // PZ8QcGq7/hJPGGnA/NBD4xFaIE/qYnuvqhnIKzclLb5l
// SIG // oRKKJQ9jo/dUHPkhydYV81KsbkMyB/2CF/jlZ2wNUfa9
// SIG // 8VLHvefEMPwgMQmIHZUpGk3VHQKl8YDgA7Rb9LHdyFfu
// SIG // ZUnHUlS2tAMoEv+Q1vAIj364l8WrNyzkeuSod+N2oADQ
// SIG // aj/B0jaK4EESqDVqG2rbNeHUHATkqEUEyFozOG5NHA1i
// SIG // twqijNPVVD9GzRxVpnDbEjqHk3Wfp9KgMIIEyTCCA7Gg
// SIG // AwIBAgIQaguZT8AA3qoR1NhAmqi+5jANBgkqhkiG9w0B
// SIG // AQQFADBwMSswKQYDVQQLEyJDb3B5cmlnaHQgKGMpIDE5
// SIG // OTcgTWljcm9zb2Z0IENvcnAuMR4wHAYDVQQLExVNaWNy
// SIG // b3NvZnQgQ29ycG9yYXRpb24xITAfBgNVBAMTGE1pY3Jv
// SIG // c29mdCBSb290IEF1dGhvcml0eTAeFw0wMDEyMTAwODAw
// SIG // MDBaFw0wNTExMTIwODAwMDBaMIGmMQswCQYDVQQGEwJV
// SIG // UzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMH
// SIG // UmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBv
// SIG // cmF0aW9uMSswKQYDVQQLEyJDb3B5cmlnaHQgKGMpIDIw
// SIG // MDAgTWljcm9zb2Z0IENvcnAuMSMwIQYDVQQDExpNaWNy
// SIG // b3NvZnQgQ29kZSBTaWduaW5nIFBDQTCCASAwDQYJKoZI
// SIG // hvcNAQEBBQADggENADCCAQgCggEBAKKEFVPYCzAONJX/
// SIG // OhvC8y97bTcjTfPSjOX9r/3FAjQfJMflodxU7H4CdEer
// SIG // 2zJYFhRRKTjxfrK0jDpHtTlOblTCMQw6bfvNzctQnBuu
// SIG // p9jZSiY/tcXLj5biSfJt2OmWPt4Fz/CmVTetL2DNgGFC
// SIG // oUlUSg8Yt0vZk5kwWkd1ZLTTu922qwydT7hzOxg6qrSH
// SIG // jLCIsE1PH04RtTOA3w06ZG9ExzS9SpObvKYd+QUjTmAp
// SIG // j8wq8oSama2o2wpwe9Y0QZClt2bHXBsdozMOm1QDGj+Y
// SIG // kLjM5z0EdEMcj/c55rOsSHprKg5iAWE5dm79PpgHSxTx
// SIG // AUb9FQDgR9pP5AXkgCUCAQOjggEoMIIBJDATBgNVHSUE
// SIG // DDAKBggrBgEFBQcDAzCBogYDVR0BBIGaMIGXgBBb0HDv
// SIG // aXKeI1F+FLJNjv/LoXIwcDErMCkGA1UECxMiQ29weXJp
// SIG // Z2h0IChjKSAxOTk3IE1pY3Jvc29mdCBDb3JwLjEeMBwG
// SIG // A1UECxMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSEwHwYD
// SIG // VQQDExhNaWNyb3NvZnQgUm9vdCBBdXRob3JpdHmCDwDB
// SIG // AIs8PIgR0T72Y+zfQDAQBgkrBgEEAYI3FQEEAwIBADAd
// SIG // BgNVHQ4EFgQUKVy5G7bNM+67nll99+XKLsQNNCgwGQYJ
// SIG // KwYBBAGCNxQCBAweCgBTAHUAYgBDAEEwCwYDVR0PBAQD
// SIG // AgFGMA8GA1UdEwEB/wQFMAMBAf8wDQYJKoZIhvcNAQEE
// SIG // BQADggEBAEVY4ppBf/ydv0h3d66M2eYZxVe0Gr20uV8C
// SIG // oUVqOVn5uSecLU2e/KLkOIo4ZCJC37kvKs+31gbK6yq/
// SIG // 4BqFfNtRCD30ItPUwG2IgRVEX2SDZMSplCyK25A3Sg+3
// SIG // 6NRhj3Z24dkl/ySElY0EVlSUoRw6PoK87qWHjByMS3lf
// SIG // tUn6XjJpOh9UrXVN32TnMDzbZElE+/vEHEJx5qA9Re5r
// SIG // AJ+sQr26EbNW5PvVoiqB2B9OolW+J49wpqJsG/9UioK8
// SIG // gUumobFmeqkXp8sGwEfrprPpMRVTPSoEv/9zSNyLJ0P8
// SIG // Y+juJIdbvjbR6DH1Mtle33l6ujCsaYZK+4wRvxuNVFkw
// SIG // ggUPMIID96ADAgECAgphBxFDAAAAAAA0MA0GCSqGSIb3
// SIG // DQEBBQUAMIGmMQswCQYDVQQGEwJVUzETMBEGA1UECBMK
// SIG // V2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwG
// SIG // A1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSswKQYD
// SIG // VQQLEyJDb3B5cmlnaHQgKGMpIDIwMDAgTWljcm9zb2Z0
// SIG // IENvcnAuMSMwIQYDVQQDExpNaWNyb3NvZnQgQ29kZSBT
// SIG // aWduaW5nIFBDQTAeFw0wMjA1MjUwMDU1NDhaFw0wMzEx
// SIG // MjUwMTA1NDhaMIGhMQswCQYDVQQGEwJVUzETMBEGA1UE
// SIG // CBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEe
// SIG // MBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSsw
// SIG // KQYDVQQLEyJDb3B5cmlnaHQgKGMpIDIwMDIgTWljcm9z
// SIG // b2Z0IENvcnAuMR4wHAYDVQQDExVNaWNyb3NvZnQgQ29y
// SIG // cG9yYXRpb24wggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAw
// SIG // ggEKAoIBAQCqmb05qBgn9Cs9C0w/fHcup8u10YwNwjp0
// SIG // 15O14KBLP1lezkVPmnkp8UnMGkfuVcIIPhIg+FXy7l/T
// SIG // 4MqWvDDe/ljIJzLQhVTo8JEQu/MrvhnlA5sLhh3zsDmM
// SIG // uP0LHTxzJqxXK8opohWQghXid6NAUgOLncJwuh/pNPbz
// SIG // NZJOVYP42jC2IN5XBrVaQgbeWcvy36a9FUdxGSUj0stv
// SIG // mxl532pb8XYFeSn8w1bKj0QIhVWKy8gPRktVy4yWd0qH
// SIG // 6KlBBsf/DeloV2Nyw2lXtEPPMjow3Bvp1UMmKnn+ldsi
// SIG // ZyTJL9A04+b7UUmGuDzQJV/W7J4DYYepaEDH+OID5s8F
// SIG // AgMBAAGjggFAMIIBPDAOBgNVHQ8BAf8EBAMCBsAwEwYD
// SIG // VR0lBAwwCgYIKwYBBQUHAwMwHQYDVR0OBBYEFGvIxlEg
// SIG // 8LQv06C2rn9eJrK4h1IpMIGpBgNVHSMEgaEwgZ6AFClc
// SIG // uRu2zTPuu55Zffflyi7EDTQooXSkcjBwMSswKQYDVQQL
// SIG // EyJDb3B5cmlnaHQgKGMpIDE5OTcgTWljcm9zb2Z0IENv
// SIG // cnAuMR4wHAYDVQQLExVNaWNyb3NvZnQgQ29ycG9yYXRp
// SIG // b24xITAfBgNVBAMTGE1pY3Jvc29mdCBSb290IEF1dGhv
// SIG // cml0eYIQaguZT8AA3qoR1NhAmqi+5jBKBgNVHR8EQzBB
// SIG // MD+gPaA7hjlodHRwOi8vY3JsLm1pY3Jvc29mdC5jb20v
// SIG // cGtpL2NybC9wcm9kdWN0cy9Db2RlU2lnblBDQS5jcmww
// SIG // DQYJKoZIhvcNAQEFBQADggEBADUj/RNU/Onc8N0MFHr6
// SIG // p7PO/ac6yLrl5/YD+1Pbp5mpoJs2nAPrgkccIb0Uy+dn
// SIG // QAnHFpECVc5DQrTNG12w8zIEPRLlHacHp4+jfkVVdhuW
// SIG // lZFp8N0480iJ73BAt9u1VYDAA8QutijcCoIOx0Pjekhd
// SIG // uAaJkkBsbsXc+JrvC74hCowvOrXtp85xh2gj4bPkGH24
// SIG // RwGlK8RYy7KJbF/90yzEb7gjsg3/PPIRRXTyCQaZGN1v
// SIG // wIYBGBIdKxavVu9lM6HqZ070S4Kr6Q/cAfrfYH9mR13L
// SIG // LHDMe07ZBrhujAz+Yh5C+ZN8oqsKntAjEK5NeyeRbya+
// SIG // aPqmP58j68idu4cxggTaMIIE1gIBATCBtTCBpjELMAkG
// SIG // A1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAO
// SIG // BgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29m
// SIG // dCBDb3Jwb3JhdGlvbjErMCkGA1UECxMiQ29weXJpZ2h0
// SIG // IChjKSAyMDAwIE1pY3Jvc29mdCBDb3JwLjEjMCEGA1UE
// SIG // AxMaTWljcm9zb2Z0IENvZGUgU2lnbmluZyBQQ0ECCmEH
// SIG // EUMAAAAAADQwCQYFKw4DAhoFAKCBqjAZBgkqhkiG9w0B
// SIG // CQMxDAYKKwYBBAGCNwIBBDAcBgorBgEEAYI3AgELMQ4w
// SIG // DAYKKwYBBAGCNwIBFTAjBgkqhkiG9w0BCQQxFgQURKkO
// SIG // QXb9/8Gu4J/yqzBPWkkwJI4wSgYKKwYBBAGCNwIBDDE8
// SIG // MDqgGIAWAG8AdwBzAGIAcgBvAHcAcwAuAGoAc6EegBxo
// SIG // dHRwOi8vb2ZmaWNlLm1pY3Jvc29mdC5jb20gMA0GCSqG
// SIG // SIb3DQEBAQUABIIBAC6DQJ1CpMz4Dse72ABumRjqaU3z
// SIG // gxw0TT0n36C9YW45jwoOsRzaZOwLwFQtlfeg2YqA9jf6
// SIG // 2hi3/GTk38HC77UR5XxoinZSKVxhbOIQT0sF9FHuREoX
// SIG // UChJ5MCbgcbS5lvIPOP0MCjEvcSMkLUN2MWu1zd/XVl4
// SIG // 6JTBKjKQyQOoD1VU4DKIp4NShycyUz999eV1XR3OJrlJ
// SIG // XOnnhrJiusHjL116R4s+Ze9M5jgoicjqRRp3UXxT9Quu
// SIG // DidQIvTouYSp3bevTS88CENBZAqzcAqX9XDi7xS1WAsE
// SIG // jfGEifl1yqVSzoHKIVZ0zITJE26qHq963wIFxYFH2hSe
// SIG // oADGjlShggJMMIICSAYJKoZIhvcNAQkGMYICOTCCAjUC
// SIG // AQEwgbMwgZ4xHzAdBgNVBAoTFlZlcmlTaWduIFRydXN0
// SIG // IE5ldHdvcmsxFzAVBgNVBAsTDlZlcmlTaWduLCBJbmMu
// SIG // MSwwKgYDVQQLEyNWZXJpU2lnbiBUaW1lIFN0YW1waW5n
// SIG // IFNlcnZpY2UgUm9vdDE0MDIGA1UECxMrTk8gTElBQklM
// SIG // SVRZIEFDQ0VQVEVELCAoYyk5NyBWZXJpU2lnbiwgSW5j
// SIG // LgIQCHptXG9ik0+6xP1D4RQYnTAMBggqhkiG9w0CBQUA
// SIG // oFkwGAYJKoZIhvcNAQkDMQsGCSqGSIb3DQEHATAcBgkq
// SIG // hkiG9w0BCQUxDxcNMDMwNzE1MDYwNDQ0WjAfBgkqhkiG
// SIG // 9w0BCQQxEgQQM3Bxt6gfpNY17LEzzRwfhzANBgkqhkiG
// SIG // 9w0BAQEFAASCAQAMwR5EZPMBV0Mlobpl0lMYz3zWPwu7
// SIG // rNh9Pg3JISPm9niP12b6mj0baOIz6zLoKd3XPNtf2CjO
// SIG // gJWplq46NqhaMXkNmB5N1PEFnmWjFFL1f2oNhAvR+vDD
// SIG // 4Mm8n7gKufm9gkuniPet5e2Tq+6tYv3vsyKWu6istRzD
// SIG // 3wDRH1j3gTTm+yepkZ1p1Gx2FUwBNDvGd4pttvnYLTLV
// SIG // W4ozNDC8YRo5QzSIvUrrzrADW1VRyHKBHr8r4W9hEAqB
// SIG // ondQlJ6ola7Hq8eBy+W/rRIJ0Paip3PxG/QWFXxom31S
// SIG // fEQ3oNf9xqoIkx2+bzgBM8COeI7AwcAS1bRwJzAV1Imh
// SIG // aHbm
// SIG // End signature block
