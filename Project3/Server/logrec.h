//---------------------------------------------------------------------------

#ifndef logrecH
#define logrecH
#include<system.hpp>
#include<fstream.h>
#include<vcl.h>
//---------------------------------------------------------------------------
/*��־��¼*/
void logrec(const String &Text, const String &Addr);
void logError(const String &option, const String &Addr, const String &Error);
void logAuto(const String &App, const String &Error);
#endif
