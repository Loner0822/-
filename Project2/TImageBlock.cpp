//---------------------------------------------------------------------------


#pragma hdrstop

#include "TImageBlock.h"


//---------------------------------------------------------------------------

#pragma package(smart_init)
TImageBlock::TImageBlock()
{
    DataHead = new TParamGroup("DataHead");
    DrawParam = new TParamGroup("DrawParam");
    LocParam = new TParamGroup("Loc");

    UserPropertyList = new TObjectList();
    controlList = new TObjectList();
    connectionList = new TObjectList();

    Geometry = new TParamGroup("Geometry");
    LineFormat = new TParamGroup("LineFormat");
    FillFormat = new TParamGroup("FillFormat");
    TextFormat = new TParamGroup("TextFormat");
    ShapeCount = new TParamGroup("ShapeCount");

    nextBlocks = new TObjectList();
    otherBlocks = new TObjectList();

    DrawParamType = 0;
}

AnsiString TImageBlock::getID()
{
    return DataHead->getValueByName("ID");
}

int TImageBlock::getConnectionCount()
{
    return connectionList->Count;
}

AnsiString TImageBlock::getToId( int i )
{
    if( i >= connectionList->Count )
    {
        return "";
    }
    TParamGroup *gp = (TParamGroup *)connectionList->Items[i];
    return gp->getValueByName("toId").ToInt();
}

bool TImageBlock::HaveConnPoint( AnsiString id )
{
    for( int i = 0; i < connectionList->Count; i++ )
    {
        if( getToId(i) == id )
        {
            return true;
        }
    }
    return false;
}

void TImageBlock::analyzeNextBlock( TObjectList *AllBlocks )
{
    if( nextBlocks->Count > 0)
    {
        return;
    }
    int count = AllBlocks->Count;

    for( int i = 0; i< connectionList->Count ; i ++ )
    {
        TImageBlock *block = getBlock( getToId(i), AllBlocks );
        nextBlocks->Add(block);
    }

    for( int i = 0; i< count; i ++ )
    {
        TImageBlock *block = (TImageBlock *)AllBlocks->Items[i];
        AnsiString id = DataHead->getValueByName("ID");
        int conncount = block->getConnectionCount();
        for( int i =0; i < conncount; i ++  )
        {
            AnsiString toid = block->getToId(i);
            if( id == toid )
            {
                nextBlocks->Add(block);
            }
        }
    }
}

int TImageBlock::getNextBlockCount()
{
    return nextBlocks->Count;
}

TImageBlock *TImageBlock::getNextBlock( int i )
{
    if( i >= nextBlocks->Count )
    {
        return NULL;
    }
    return (TImageBlock *)nextBlocks->Items[i];
}

TImageBlock *TImageBlock::getBlock( AnsiString id,TObjectList *AllBlocks  )
{
    int count = AllBlocks->Count;
    for( int i = 0; i< count; i ++ )
    {
        TImageBlock *block = (TImageBlock *)AllBlocks->Items[i];
        AnsiString id1 = block->DataHead->getValueByName("ID");
        if( id1 == id )
        {
            return block;
        }
    }    
}

bool TImageBlock::HaveRepeat( TImageBlock *other, TObjectList *line )
{
    AnsiString id = other->DataHead->getValueByName("ID");
    for( int i = 0; i < line->Count - 1; i++ )
    {
        TImageBlock *block = (TImageBlock *)line->Items[i];
        AnsiString id1 = block->DataHead->getValueByName("ID");
        if( id1 == id )
        {
            return true;
        }
    }
    return false;
}
//从0开始，0。1。2。3   2维图标
TImageBlock *TImageBlock::getNextBlockById( int conid )
{
    AnsiString id = DataHead->getValueByName("ID");
    if( connectionList->Count == 0 )
    {
        int count = nextBlocks->Count;
        for( int i = 0 ; i < count; i ++ )
        {
            TImageBlock *next = (TImageBlock *)nextBlocks->Items[i];
            int concount = next->connectionList->Count;
            for( int j = 0; j < concount; j ++ )
            {
                TParamGroup *gp = (TParamGroup *)next->connectionList->Items[j];
                AnsiString toid = gp->getValueByName("toId");
                AnsiString toRow = gp->getValueByName("toRow");
                if( toid == id && toRow == conid )
                {
                    return next;
                }
            }
        }
    }else
    {
        logrec( "error" + id );
    }
}
//对于1D图形
bool TImageBlock::getPostion( AnsiString toId, float &x, float &y )
{
    if( connectionList->Count == 0 )
    {
        return false;
    }else
    {
        int concount = connectionList->Count;
        for( int j = 0; j < concount; j ++ )
        {
            TParamGroup *gp = (TParamGroup *)connectionList->Items[j];
            AnsiString toid1 = gp->getValueByName("toId");
            AnsiString fromRow = gp->getValueByName("fromRow");
            if( toid1 == toId )
            {
                if( fromRow == "100" )
                {
                    x = DrawParam->getValueByName("beginX").ToDouble();
                    y = DrawParam->getValueByName("beginY").ToDouble();
                }
                if( fromRow == "102" )
                {
                    x = DrawParam->getValueByName("endX").ToDouble();
                    y = DrawParam->getValueByName("endY").ToDouble();
                }
            }
        }
    }
}

int TImageBlock::getToRow( TImageBlock *oneD, TImageBlock *twoD )
{
    AnsiString twoDid = twoD->DataHead->getValueByName("ID");
    int count = oneD->connectionList->Count;
    for( int i = 0; i < count; i++ )
    {
         TParamGroup *gp = (TParamGroup *)oneD->connectionList->Items[i];
         AnsiString toid = gp->getValueByName("toId");
         AnsiString toRow = gp->getValueByName("toRow");
         if( toid == twoDid )
         {
            return toRow.ToInt();
         }
    }
}






