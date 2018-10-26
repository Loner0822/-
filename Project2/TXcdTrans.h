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
    
    TObjectList *DataBlockList;   //ѭ�� ���ݿ� �б�

private:
    int DataBlockTranslate( char*& binaryData, int begin );
    int getDarwParamType( char* flag );//���ͼ�λ�������������
    TParamGroup *getDrawParam( TParamGroup *DataHead ); //��������ͷflag�жϽṹ������

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
