//---------------------------------------------------------------------------

#ifndef logrecH
#define logrecH
#include<system.hpp>
#include<fstream.h>
#include<vcl.h>
//---------------------------------------------------------------------------
/*日志记录*/
void logrec( const AnsiString &Content );
/*字符串拆分*/
TStringList * SplitStr( const AnsiString &str, const AnsiString &SplitFlag);
#endif
