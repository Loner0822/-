//---------------------------------------------------------------------------

#ifndef TXcdTransH
#define TXcdTransH
#include <vcl.h>
#include "TPubFunc.h"
#include "TDataStruct.h"
//---------------------------------------------------------------------------
class TXcdTrans
{
private:
    char *OrgChars;
    long length;

    int tmpBegin;
    int tmpEnd;

    TParamGroup *RiffHead;
    TParamGroup *DrawHead;
    TParamGroup *DataSig;
    
    TObjectList *DataBlockList;   //循环 数据块 列表

private:
    int DataBlockTranslate( char*& binaryData, int begin );
    int getDarwParamType( char* flag );//获得图形基本参数的类型
    TParamGroup *getDrawParam( TParamGroup *DataHead ); //根据数据头flag判断结构体类型

public:
    TXcdTrans();
    ~TXcdTrans(){};
    void ParsFieldInfo( char*& binaryData, long length );

    int getSize( char*& binaryData, int begin );

    TObjectList *getAllBlocks();

    TParamGroup *getDrawHead(){ return DrawHead; };
    void GetXmlText(String &xmltext);
};
extern TXcdTrans *xcdTranslater;

#endif
