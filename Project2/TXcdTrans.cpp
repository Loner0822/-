//---------------------------------------------------------------------------


#pragma hdrstop

#include "TXcdTrans.h"
#include "TPubFunc.h"
#include "logrec.h"
//---------------------------------------------------------------------------

#pragma package(smart_init)

TXcdTrans *xcdTranslater = NULL;
TXcdTrans::TXcdTrans()
{
    RiffHead = new TParamGroup("RiffHead");
    DrawHead = new TParamGroup("DrawHead");
    DataSig = new TParamGroup("DataSig");
    DataBlockList = new TObjectList();
    DataBlockList->Clear();
}

void TXcdTrans::ParsFieldInfo( char*& binaryData, long length )
{
    if( length <= 0 )
    {
        logrec( "字符串空" );
        return ;
    }else
    {
        this->length = length;
    }
    //logrec("--------------------------------------------");
    int DrawBeginPos = RiffHead->Translate( binaryData, 0, length );

    int DatasigBeginPos = DrawHead->Translate( binaryData, DrawBeginPos, length );

    int BlockBeginPos = DataSig->Translate( binaryData, DatasigBeginPos, length );

//开始数据块解析 循环
    int tmpPos = BlockBeginPos;
    while(1)
    {
        if( tmpPos >= length )
        {
            break;
        }
        int size = getSize( binaryData, tmpPos ); //数据块大小
        if(size ==0)
        {
           break;
        }
        //logrec("----------------------------------------------------------");
        DataBlockTranslate( binaryData, tmpPos ); //数据块解析
        tmpPos += size;
    }
}
#include "TImageBlock.h"
int TXcdTrans::DataBlockTranslate( char*& binaryData, int begin )
{
    TImageBlock *block = new TImageBlock();
    
    int beginPos = begin;
    int tmpPos = begin;
    //解析datahead
    //logrec( "datahead----------" );
    TParamGroup *DataHead = new TParamGroup("DataHead");
    tmpPos = DataHead->Translate( binaryData, tmpPos, length );
    block->DataHead = DataHead;
    //解析基本参数
    TParamGroup *DrawParam = getDrawParam( DataHead );
    tmpPos = DrawParam->Translate( binaryData, tmpPos , length, DataHead );
    block->DrawParam = DrawParam;
    
    int type = getDarwParamType( DataHead->getCValueByName("flag") );

    block->DrawParamType = type;
    
    if( type == 1 || type == 2 )
    {
        AnsiString s = DrawParam->getParamByName("text")->s_value;
        int iconid = DrawParam->getParamByName("iconID")->s_value.ToIntDef(0);

        int size = DataHead->getParamByName("size")->s_value.ToIntDef(0);
        int size2 = DataHead->getParamByName("size2")->s_value.ToIntDef(0);

        if( iconid < 0 )
        {
            delete block;
            return beginPos + size + size2;
        }
    }
    {
        int userPropertyCount = DataHead->getParamByName("userPropertyCount")->s_value.ToIntDef(0);
        //int controlCount = DataHead->getParamByName("controlCount")->s_value.ToIntDef(0);
       // int connectionCount = DataHead->getParamByName("connectionCount")->s_value.ToIntDef(0);
        for( int i = 0; i < userPropertyCount; i++ )
        {
            //logrec( "UserProperty--------" );
            TParamGroup *userProperty = new TParamGroup("UserProperty");
            tmpPos = userProperty->Translate( binaryData, tmpPos,length );
            block->UserPropertyList->Add( userProperty );
        }
        /*
        for( int i = 0; i < controlCount; i++ )
        {
            logrec( "ControlPoint--------" );
            TParamGroup *control = new TParamGroup("ControlPoint");
            tmpPos = control->Translate( binaryData, tmpPos,length );
            block->controlList->Add(control);
        }
        for( int i = 0; i < connectionCount; i++ )
        {
            logrec( "ConnectionData--------" );
            TParamGroup *connection = new TParamGroup("ConnectionData");
            tmpPos = connection->Translate( binaryData, tmpPos,length );
            block->connectionList->Add( connection );
        }
        if( type == 4 || type == 2 )
        {
            TParamGroup *loc = new TParamGroup("Loc");
            tmpPos = loc->Translate( binaryData, tmpPos,length, DataHead, DrawParam );
            block->LocParam = loc;
        } */

    }
    /*
    {
        char *flagEx = DataHead->getCValueByName("flagEx");


         TParamGroup *Geometry = new TParamGroup("Geometry");
         if( GetBitValue( *flagEx, 2 ) )
         {
            logrec( "Geometry--------" );
            tmpPos = Geometry->Translate( binaryData, tmpPos,length );
            block->Geometry = Geometry;
         }

         TParamGroup *LineFormat = new TParamGroup("LineFormat");
         if( GetBitValue( *flagEx, 3 ) )
         {
            logrec( "LineFormat--------" );
            tmpPos = LineFormat->Translate( binaryData, tmpPos,length );
            block->LineFormat = LineFormat;
         }

         TParamGroup *FillFormat = new TParamGroup("FillFormat");
         if( GetBitValue( *flagEx, 4 ) )
         {
            logrec( "FillFormat--------" );
            tmpPos = FillFormat->Translate( binaryData, tmpPos,length );
            block->FillFormat = FillFormat;
         }

         TParamGroup *TextFormat = new TParamGroup("TextFormat");
         if( GetBitValue( *flagEx, 5 ) )
         {
            logrec( "TextFormat--------" );
            tmpPos = TextFormat->Translate( binaryData, tmpPos,length );
            block->TextFormat = TextFormat;
         }

         //子图形数量

         TParamGroup *ShapeCount = new TParamGroup("ShapeCount");
         if( GetBitValue( *flagEx, 6 ) )
         {
            logrec( "ShapeCount--------" );
            tmpPos = ShapeCount->Translate( binaryData, tmpPos,length );
            block->ShapeCount = ShapeCount;
         }
         int count = ShapeCount->getParam("0")->s_value.ToIntDef(0);
         for( int i =0; i < count; i ++ )
         {
            
            tmpPos = DataBlockTranslate( binaryData, tmpPos );
         }
    }
    */
    DataBlockList->Add( block );
    return tmpPos;
}

TParamGroup *TXcdTrans::getDrawParam( TParamGroup *DataHead )
{
    TParamGroup *DrawParam;
    int type = getDarwParamType( DataHead->getCValueByName( "flag" ) );
    switch( type )
    {
        case 1:
            //logrec("OneDIconData---------");
            DrawParam = new TParamGroup("OneDIconData");
            break;
        case 2:
            //logrec("IconData---------");
            DrawParam = new TParamGroup("IconData");
            break;
        case 3:
            //logrec("OneDShapeData---------");
            DrawParam = new TParamGroup("OneDShapeData");
            break;
        case 4:
            //logrec("ShapeData---------");
            DrawParam = new TParamGroup("ShapeData");
            break;
        default:
            DrawParam = NULL;
    }
    return DrawParam;
}

int TXcdTrans::getDarwParamType( char* flag )
{
    int IsIcon = GetBitValue( *flag, 0 );
    int Is2D = GetBitValue( *flag, 1 );
    if( !IsIcon && !Is2D ) //全为0  1D图形
        return 3;
    if( IsIcon && Is2D )  //全为1  2D图标
        return 2;
    if( !IsIcon && Is2D ) // 2D 图形
        return 4;
    if( IsIcon && !Is2D ) // 1D图标
        return 1;
}

int TXcdTrans::getSize( char*& binaryData, int begin )
{
    char *size;
    int blocksize = 0;
    if( !CutCharStr(binaryData, size, begin, begin+1, length))
    {
        return 0;
    }
    blocksize = *((BYTE*)size);
    delete []size;
    if( blocksize == 0 )
    {
        if(CutCharStr(binaryData, size, begin+8, begin+10, length))
        {
            blocksize = *((short*)size) + 2;
            delete []size;
            if( blocksize == 0 )
            {
                if(CutCharStr(binaryData, size, begin+10, begin+14, length))
                {
                    blocksize = *((DWORD*)size) + 6;
                    delete[] size;
                }
            }
        }
    }
    return blocksize;
}

TObjectList *TXcdTrans::getAllBlocks()
{
    return DataBlockList;
}
void TXcdTrans::GetXmlText(String &xmltext)
{
      TObjectList *Blocks = getAllBlocks();
      int count = Blocks->Count;
      AnsiString xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"  ;
      xml += "<UserProperty>";
      for( int i = 0; i < count; i++ )
      {
          TImageBlock *block =  (TImageBlock *)Blocks->Items[i];
          if( block->DrawParamType == 1 || block->DrawParamType == 2 )//图标
          {
               AnsiString iconid = block->DrawParam->getValueByName("iconID").Trim();
               AnsiString id = block->DataHead->getValueByName("ID").Trim();
               TParamGroup *userProperty;
               AnsiString ss ;
               String icon_id = "";
               String id_count  = "";
               try
               {
                   if(block->UserPropertyList->Count > 0)
                   {
                         userProperty = (TParamGroup *)block->UserPropertyList->Items[0];
                         ss = userProperty->getValueByName("value");
                         ss = StringReplace(ss, "\"", "", TReplaceFlags()<<rfReplaceAll);
                         if( block->UserPropertyList->Count > 1)
                         {
                               for(int i = 1;i <block->UserPropertyList->Count;i++ )
                               {
                                       userProperty = (TParamGroup *)block->UserPropertyList->Items[i];
                                       String str = userProperty->getValueByName("value");
                                       str = StringReplace(str, "\"", "", TReplaceFlags()<<rfReplaceAll);
                                       if(str!="")
                                       {
                                             ss += ","+str ;
                                       }
                               }
                         }
                   }
                   icon_id = "id_"+id;
                   id_count = ss;
               }
               catch(...)
               {
               }
               if(ss!="")
               {
                    xml += "<"+icon_id+">"+id_count +"</"+icon_id+">";
               }
          }
      }
      xml += "</UserProperty>";
      xmltext = xml;
}

