//---------------------------------------------------------------------------

#ifndef TImageBlockH
#define TImageBlockH
#include <vcl.h>
#include "TDataStruct.h"
#include "TPubFunc.h"
#include "logrec.h"
//---------------------------------------------------------------------------
class TImageBlock : public TObject
{
public:
    TParamGroup *DataHead;
    TParamGroup *DrawParam;
    TParamGroup *LocParam;
    TObjectList *UserPropertyList;
    TObjectList *controlList;
    TObjectList *connectionList;

    TParamGroup *Geometry;
    TParamGroup *LineFormat;       
    TParamGroup *FillFormat;
    TParamGroup *TextFormat;
    TParamGroup *ShapeCount;

    int DrawParamType;

    TObjectList *nextBlocks;

    TObjectList *otherBlocks;  //��ʱ��ÿ��ʹ��ǰ������
public:
    TImageBlock();
    __fastcall ~TImageBlock(){};
    AnsiString getID();
    int getConnectionCount();
    AnsiString getToId( int i );
    
    bool HaveConnPoint( AnsiString id );
public:

    void analyzeNextBlock( TObjectList *AllBlocks );

    //����
    int getNextBlockCount();

    TImageBlock *getNextBlock( int i );
//����2Dͼ������ӵ��Ż�ȡ����ͼ��
    TImageBlock *getNextBlockById( int conid );
//�������ӵ�2Dͼ��ID����ȡ���ӵ�
    bool getPostion( AnsiString toId, float &x, float &y );
//��ȡ1Dͼ������2Dͼ�����ӵ���
    static int getToRow( TImageBlock *oneD, TImageBlock *twoD );

    static TImageBlock *getBlock( AnsiString id, TObjectList *AllBlocks );

    static bool HaveRepeat( TImageBlock *other, TObjectList *line );
};

#endif
