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

    TObjectList *otherBlocks;  //临时，每次使用前先生成
public:
    TImageBlock();
    __fastcall ~TImageBlock(){};
    AnsiString getID();
    int getConnectionCount();
    AnsiString getToId( int i );
    
    bool HaveConnPoint( AnsiString id );
public:

    void analyzeNextBlock( TObjectList *AllBlocks );

    //相邻
    int getNextBlockCount();

    TImageBlock *getNextBlock( int i );
//根据2D图标的连接点编号获取连接图符
    TImageBlock *getNextBlockById( int conid );
//根据连接的2D图标ID来获取连接点
    bool getPostion( AnsiString toId, float &x, float &y );
//获取1D图形连接2D图形连接点编号
    static int getToRow( TImageBlock *oneD, TImageBlock *twoD );

    static TImageBlock *getBlock( AnsiString id, TObjectList *AllBlocks );

    static bool HaveRepeat( TImageBlock *other, TObjectList *line );
};

#endif
