//---------------------------------------------------------------------------


#pragma hdrstop
#include "TAdvStringGridCenter.h"

//---------------------------------------------------------------------------

#pragma package(smart_init)
TAdvStringGridCenter::TAdvStringGridCenter(TMemo * amemo)
{
      memo = amemo ;
}
TAdvStringGridCenter::~TAdvStringGridCenter()
{

}
void TAdvStringGridCenter::AutoWordBreakAndCenter(TAdvStringGrid * sg,int ACol,int ARow)
{
      String  value = sg->Cells[ACol][ARow];
      value = StringReplace(value, "\r\n", "", TReplaceFlags()<<rfReplaceAll);
      value = StringReplace(value, "\n", "", TReplaceFlags()<<rfReplaceAll);
      value = StringReplace(value, "\r", "", TReplaceFlags()<<rfReplaceAll);
      if( !memo )
      {
           return;
      }

    //  if(value.Length() > 0)

      {
         if( !sg->IsMergedCell(ACol,ARow))
         {

              //  memo->Lines->Clear();
                memo->Font =  sg->Font;
                memo->Width = sg->ColWidths[ACol];
                memo->Text = value;
              //  memo->Lines->Add(value);
                //清空网格
                TRect r = sg->CellRect(ACol,ARow);
                sg->Canvas->FillRect(TRect(r.left,r.top,r.right-1,r.bottom-1));
                //设置行高
                int h = sg->Canvas->TextHeight(memo->Lines->Strings[0]);
                if( sg->RowHeights[ARow]< memo->Lines->Count * h + 4)
                {
                    sg->RowHeights[ARow] = memo->Lines->Count * h + 4;
                }
                //重绘
                sg->Canvas->Pen->Color = clBlack;
                int y = r.top + (sg->RowHeights[ARow] - memo->Lines->Count * h) / 2;//垂直居中
                for(int i = 0;i<memo->Lines->Count;i++)
                {
                   int w = sg->Canvas->TextWidth(memo->Lines->Strings[i]);
                   int x = r.left + (sg->ColWidths[ACol] - w) / 2 ;  //水平居中
                   sg->Canvas->TextOutA(x,y+h*i,memo->Lines->Strings[i]);
                }
         }
         else
         {
         //return;
              if( sg->IsMergedNonBaseCell(ACol,ARow))
              {}
              else
              {
                     TRect r = sg->CellRect(ACol,ARow);
                     int width = r.Width() ;
                     int height = r.Height() ;
                     
                   //  memo->Lines->Clear();
                     memo->Font =  sg->Font;
                     memo->Width = width;
                   //  memo->Lines->Add(value);
                     memo->Text = value;

                     sg->Canvas->FillRect(TRect(r.left,r.top,r.right-1,r.bottom-1));
                     int h = sg->Canvas->TextHeight(memo->Lines->Strings[0]);
                     if( height > memo->Lines->Count * h + 4)
                     {
                            sg->Canvas->Pen->Color = clBlack;
                            int y = r.top + (height - memo->Lines->Count * h) / 2;
                            for(int i = 0;i<memo->Lines->Count;i++)
                            {
                                 int w = sg->Canvas->TextWidth(memo->Lines->Strings[i]);
                                 int x = r.left + (width - w) / 2 ;
                                 sg->Canvas->TextOutA(x,y+h*i,memo->Lines->Strings[i]);
                            }
                     }
                     else
                     {
                         int k =  height/ sg->RowHeights[ARow];
                         int kh = ( memo->Lines->Count * h + 4 - height) / k  ;
                         for( int i = 0 ;i < k ;i++ )
                         {
                             sg->RowHeights[ARow+i] = sg->RowHeights[ARow] + kh;
                         }
                         TRect r0 = sg->CellRect(ACol,ARow);
                         height = r0.Height() ;

                         //sg->Canvas->FillRect(TRect(r0.left,r0.top,r0.right-1,r0.bottom-1));

                         sg->Canvas->Pen->Color = clBlack;
                         int y = r.top + (height - memo->Lines->Count * h) / 2;
                         for(int i = 0;i<memo->Lines->Count;i++)
                         {
                               int w = sg->Canvas->TextWidth(memo->Lines->Strings[i]);
                               int x = r.left + (width - w) / 2 ;
                               sg->Canvas->TextOutA(x,y+h*i,memo->Lines->Strings[i]);
                         }

                     }

              }
         }


      } 
}

