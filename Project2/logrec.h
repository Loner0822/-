//---------------------------------------------------------------------------

#ifndef logrecH
#define logrecH
#include<system.hpp>
#include<fstream.h>
#include<vcl.h>
//---------------------------------------------------------------------------
/*��־��¼*/
void logrec( const AnsiString &Content );
/*�ַ������*/
TStringList * SplitStr( const AnsiString &str, const AnsiString &SplitFlag);
#endif
