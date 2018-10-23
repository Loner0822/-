//---------------------------------------------------------------------------

#ifndef GUIDH
#define GUIDH
//---------------------------------------------------------------------------
#include <stdio.h>
#include <vcl.h>
const char* newGUID()
{
    static char buf[64] = {0};
    GUID guid;
    if (S_OK == ::CoCreateGuid(&guid))
    {
        _snprintf(buf, sizeof(buf),
                  "{%08X-%04X-%04x-%02X%02X-%02X%02X%02X%02X%02X%02X}",
                  guid.Data1,
                  guid.Data2,
                  guid.Data3,
                  guid.Data4[0], guid.Data4[1],
                  guid.Data4[2], guid.Data4[3],
                  guid.Data4[4], guid.Data4[5],
                  guid.Data4[6], guid.Data4[7]
                    );
    }
    return (const char*)buf;
}

String Get26GuidText() {
	String guid26 = "";
	String path = ExtractFilePath(Application->ExeName) + "GuidConvert.dll";
	char * __stdcall (*Get26Guid) ();
	HINSTANCE Hdl;
	Hdl = ::LoadLibrary(path.c_str());
	if (Hdl != NULL) {
		FARPROC P;
		P = GetProcAddress(Hdl, "Get26Guid");
		if (P == NULL) {
			ShowMessage("打开Get26Guid()函数错误!");
		}
		else {
			Get26Guid = (char * __stdcall (*)())P;
			guid26 = Get26Guid();
		}
		::FreeLibrary(Hdl);
	}
	else {
		ShowMessage("不能载入DLL!");
	}
	return guid26;
}
#endif
