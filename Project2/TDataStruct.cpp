//---------------------------------------------------------------------------


#pragma hdrstop

#include "TDataStruct.h"
#include "TPubFunc.h"
#include "logrec.h"

//---------------------------------------------------------------------------

#pragma package(smart_init)

TParamGroup::TParamGroup( AnsiString flag, AnsiString flagEx )
{
    this->flag = flag;
    this->flagEx = flagEx;
    iniParamList = new TStringList();
    ParamGroupList = new TObjectList();
    Init();
}

void TParamGroup::Init()
{
    ReadIniFile();
    int count = iniParamList->Count;
    for( int i = 0; i < count ; i++ )
    {
        AnsiString name = iniParamList->Names[i];
        AnsiString value = iniParamList->Values[ iniParamList->Names[i] ];
        TStringList *list = new TStringList();
        list->CommaText = value;

        TParam pam;
        pam.name = list->Strings[0].Trim();
        pam.type = list->Strings[1].Trim();
        if( list->Count > 2 )
        {
            pam.s_value = list->Strings[2].Trim();
        }
        ParamGroup.insert( make_pair( name, pam ) );
    }
}

void TParamGroup::ReadIniFile()
{
    AnsiString path = ExtractFilePath(Application->ExeName)+"\\structs.ini" ;
    if( flag == "" )
    {
        ShowMessage( "flag 空" );
    }
    try
    {
        if(FileExists(path))
        {
            std::auto_ptr<TIniFile> file ( new TIniFile( path ) );  //绝对路径
            file->ReadSectionValues(flag, iniParamList);
        }
        else
        {
            ShowMessage("没有找到配置文件");
        }
    }
    catch(...)
    {
        ShowMessage("打开配置文件出错");
    }
}

int TParamGroup::getParamCount()
{
    return ParamGroup.size();
}

TParam* TParamGroup::getParam( const AnsiString &key )
{
    TitParamMap itmap;
    itmap = ParamGroup.find(key);

    if( itmap == ParamGroup.end() )
    {
        logrec("未找到" + key);
        return NULL;
    }
    return &itmap->second;
}

TParam* TParamGroup::getParamByName( const AnsiString &name )
{
    TitParamMap itmap;
    for( itmap = ParamGroup.begin(); itmap != ParamGroup.end(); itmap++ )
    {
        if( itmap->second.name == name )
        {
            return &itmap->second;
        }
    }
    return NULL;
}

AnsiString TParamGroup::getValueByName( AnsiString name )
{
    TitParamMap itmap  = ParamGroup.begin() ;
    for( itmap; itmap != ParamGroup.end(); itmap++ )
    {
        if( itmap->second.name == name )
        {
            return itmap->second.s_value;
        }
    }
    return "";
}

char* TParamGroup::getCValueByName( AnsiString name )
{
    TitParamMap itmap;
    for( itmap = ParamGroup.begin(); itmap != ParamGroup.end(); itmap++ )
    {
        if( itmap->second.name == name )
        {
            return itmap->second.c_value;
        }
    }
    return "";
}

void TParamGroup::AddSubStruct( TParamGroup *pg )
{
    ParamGroupList->Add(pg);
}

int TParamGroup::getSubStructCount()
{
    return ParamGroupList->Count;
}

TParamGroup *TParamGroup::getSubStruct( int i )
{
    return (TParamGroup *)ParamGroupList->Items[i];
}

int TParamGroup::Translate(  char *& org, int begin, int length, TParamGroup *groupEx,TParamGroup *groupEx1 )
{
    int tmpPos = begin;
//参数解析
    int count = getParamCount();
    for( int i = 0; i < count; i ++ )
    {
        TParam *pm = getParam(IntToStr(i));
        int paramType,fieldSize;
        fieldSize = GetTypeSize( pm->type, paramType );
        if( fieldSize >= 1000 ) //特殊处理
        {
            if( fieldSize == 1000 && paramType == 1000 ) //GValue
            {
                TParamGroup *gp = new TParamGroup( "GValue", pm->name );
                tmpPos = gp->Translate( org, tmpPos, length );
                ParamGroupList->Add(gp);
            }
            if( fieldSize == 1001 && paramType == 1001 ) //GRow[]先判断 数组大小
            {
                TParam *p = getParam(0);
                int count = p->s_value.ToIntDef(0);
                for( int i = 0 ;i < count; i++ )
                {
                    TParamGroup *gp = new TParamGroup( "GRow", pm->name );
                    tmpPos = gp->Translate( org, tmpPos, length );
                    ParamGroupList->Add(gp);
                }

            }
            if( fieldSize == 1002 && paramType == 1002 ) //char[] 先判断 数组大小
            {                                            //直接解析字符串
                if( !IsDefaultValue( pm, groupEx,groupEx1 ) )
                {
                    char* str;
                    int size;

                    if( CutCharStr( org, str, tmpPos, tmpPos + 2 ,length ) )
                    {
                        size = GetValue( str, 9 ).ToInt();  //short
                    }
                    char *str1;
                    if( CutCharStr( org, str1, tmpPos + 2, tmpPos + 2 + size ,length ) )
                    {
                        pm->s_value = GetValue( str1, 5 );  //char *
                        pm->size = size + 2;
                    }
                    tmpPos = tmpPos + size + 2;
                    if (pm->name == "ID" || pm->name == "iconID" || pm->name == "text" || pm->s_value != "")
                        logrec( pm->name + " " + pm->s_value);
                }
                if( IsDefaultValue( pm, groupEx,groupEx1 ) )
                {
                    if (pm->name == "ID" || pm->name == "iconID" || pm->name == "text" || pm->s_value != "")
                        logrec( pm->name + " " + pm->s_value);
                }
            }
        }else//普通参数
        {
            char* str;
            if( !IsDefaultValue( pm, groupEx,groupEx1 ) && CutCharStr( org, str, tmpPos, tmpPos + fieldSize ,length ) )
            {
                pm->s_value = GetValue( str, paramType );
                pm->c_value = str;
            }

            //关于size 和 size2的处理
            if( pm->name == "size2" )
            {
                AnsiString value = getValueByName("size");
                if( value == 0 )    //size为0
                {
                    if( pm->s_value.ToIntDef(0) == 0 ) //size2 2位时为零 则size2按6位处理
                    {
                        if( CutCharStr( org, str, tmpPos+2 , tmpPos + 6 ,length ) )
                        {
                            pm->size = 6;
                            pm->s_value = GetValue( str, 6 );
                        }
                        tmpPos = tmpPos + 4;
                    }
                }else
                {
                    pm->s_value = "";
                    tmpPos = tmpPos - fieldSize;
                }
            }
            //可取默认值的参数处理
            if( IsDefaultValue( pm, groupEx, groupEx1 ) )
            {
                tmpPos = tmpPos - fieldSize;
            }
            
            tmpPos = tmpPos + fieldSize;
            if (pm->name == "ID" || pm->name == "iconID" || pm->name == "text")
                logrec( pm->name + " " + pm->s_value);
        }
    }

    return tmpPos;
}

bool TParamGroup::IsDefaultValue( TParam *pm, TParamGroup *groupEx,TParamGroup *groupEx1 )
{
    AnsiString name = pm->name;
    if( groupEx && groupEx->getFlag() == "DataHead" )
    {
        char *flag = groupEx->getCValueByName("flag");
        char *flagEx = groupEx->getCValueByName("flagEx");

        //
        if( name == "height" && !GetBitValue( *flag, 4 ) )
        {
            pm->s_value = "0";
            return true;
        }
        if( name == "text" && !GetBitValue( *flag, 7 ))
        {
            pm->s_value = "";
            return true;
        }
        if( name == "angle" && !GetBitValue( *flag, 6 ) )
        {
            pm->s_value = "0";
            return true;
        }
        if( name == "width" &&  !GetBitValue( *flag, 5 ) )
        {
            pm->s_value = "0";
            return true;
        }
        if( name == "locPinX" && !GetBitValue( *flagEx, 0 ))
        {
            float f = groupEx1->getParamByName("width")->s_value.ToDouble();
            pm->s_value = FloatToStr( f*0.5 );
            return true;
        }
        if( name == "locPinY" && !GetBitValue( *flagEx, 1 ) )
        {
            float f = groupEx1->getParamByName("height")->s_value.ToDouble();
            pm->s_value = FloatToStr( f*0.5 );
            return true;
        }
    }
    if( !groupEx )
    {
        if( flag == "LineFormat" )
        {
            char *fl = getParam("0")->c_value;
            if( name == "LinePattern" && !GetBitValue( *fl, 0 ) )
            {
                pm->s_value = "1";
                return true;
            }
            if( name == "LineWeight" && !GetBitValue( *fl, 1 ) )
            {
                pm->s_value = "72";
                return true;
            }
            if( name == "LineColor" && !GetBitValue( *fl, 2 ) )
            {
                pm->s_value = "0";
                return true;
            }
            if( name == "BeginArrow" && !GetBitValue( *fl, 3 ) )
            {
                pm->s_value = "0";
                return true;
            }
            if( name == "EndArrow" && !GetBitValue( *fl, 4 ) )
            {
                pm->s_value = "0";
                return true;
            }
            if( name == "BeginArrowSize" && !GetBitValue( *fl, 5 ) )
            {
                pm->s_value = "2";
                return true;
            }
            if( name == "EndArrowSize" && !GetBitValue( *fl, 6 ) )
            {
                pm->s_value = "2";
                return true;
            }
        }
        if( flag == "TextFormat" )
        {
            char *fl = getParam("0")->c_value;
            if( name == "Font" && !GetBitValue( *fl, 0 ) )
            {
                pm->s_value = "0";
                return true;
            }
            if( name == "Size" && !GetBitValue( *fl, 1 ) )
            {
                pm->s_value = "9";
                return true;
            }
            if( name == "Color" && !GetBitValue( *fl, 2 ) )
            {
                pm->s_value = "0";
                return true;
            }
            if( name == "Style" && !GetBitValue( *fl, 3 ) )
            {
                pm->s_value = "0";
                return true;
            }
            if( name == "Case" && !GetBitValue( *fl, 4 ) )
            {
                pm->s_value = "0";
                return true;
            }
            if( name == "HAlign" && !GetBitValue( *fl, 5 ) )
            {
                pm->s_value = "1";
                return true;
            }
            if( name == "TextDirection" && !GetBitValue( *fl, 6 ) )
            {
                pm->s_value = "0";
                return true;
            }
            if( name == "TextBkgnd" && !GetBitValue( *fl, 7 ) )
            {
                pm->s_value = "0";
                return true;
            }
        }
    }
    return false;
}
