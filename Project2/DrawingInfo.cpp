
#include <Graphics.hpp>
#include "DrawingInfo.h"
#include <math>
#include <algorithm>
using namespace std;

#include "meta.h"

Graphics::TBitmap* TSigDrawing::m_defBitmap16 = 0;

static int s_hitTestAccuracy = 1;
static GetAccuracy(){
    return s_hitTestAccuracy;
}
void SetHitTestAccuracy( int accuracy ){
    if( accuracy < 1 )
        accuracy = 1;
    else if( accuracy > 20 )
        accuracy = 20;
    s_hitTestAccuracy = accuracy;
}

/*
图纸格式:
图纸文件最后4个字节 为 附加数据位置
附加数据 前2个字节为 "BN", 后面4个字节为版本
然后是 图纸ID, 图纸尺寸...
    ms->Seek( -4, soFromEnd );
    ms->Read( &pos, 4 );
    ms->Seek( pos - 1, soFromBeginning );
    char header[2];
    ms->Read( header, 2 );
    if( header[0] == 'B' && header[1] == 'N' )
    {
        int ver = 0;
        ms->Read( &ver, 4 );
        if( ver == 0xFFFF0000 + 1 )
        {
            ms->Read( &m_id, 4 );
            ms->Read( &m_box.left, 4 );
            ms->Read( &m_box.top, 4 );
            ms->Read( &m_box.right, 4 );
            ms->Read( &m_box.bottom, 4 );

            int count = 0;
            ms->Read( &count, 4 );

            m_shapes.resize( count );

            for( int i = 0; i < count; i++ )
                m_shapes[i].LoadFromStream2( ms );
        }
    }


图符类型定义:

序号  类型  类型    	图符名称

1,  	2,  	"道岔", 	"集中道岔"
2,  	2,  						"非集中道岔"
3,  	2,  						"无联锁道岔"

4,  	4,  	"信号机",	"区间信号机"
5,  	8,  						"进站信号机"
6,  	8,  						"出站信号机"
7,  	8,  						"调车信号机"
8,  	8,  						"其它信号机"
9,  	8,  						"臂板信号机"
10, 	8,  						"线路表示器"

11, 	16, 	"信号楼"
12, 	32, 	"应答器"
13, 	64, 	"电缆"
14, 	17, 	"区段"
15, 	18, 	"股道"
16, 	19, 	"区间区段"
17, 	20, 	"段管辖站场"
18, 	21, 	"段管辖区间"
19, 	22, 	"局管辖站场"
20, 	23, 	"局管辖区间"
*/
namespace
{
bool XYInRect( TRect* rc, int x, int y )
{
    TRect t(
        min(rc->left, rc->right),
        min(rc->top,  rc->bottom),
        max(rc->left, rc->right),
        max(rc->top,  rc->bottom)
        );
    ::InflateRect( &t, (GetAccuracy() - 1) * 3, (GetAccuracy() - 1) * 3 );
    return PtInRect( &t, TPoint(x,y) );
    
    if( rc->Right >= rc->Left && ::PtInRect( rc, TPoint(x,y) ) )
        return true;
    else
        return false;
}
}

TShapeInfo::TShapeInfo():m_textBBox(0, 0, -1, -1)
{
}
bool TShapeInfo::LoadFromStream( TStream* pStream )
{
    pStream->Read( &m_id, 4 );
    pStream->Read( &m_type, 4 );

    pStream->Read( &m_angle, 4 );
    pStream->Read( &x, 4 );
    pStream->Read( &y, 4 );
    pStream->Read( &width, 4 );
    pStream->Read( &height, 4 );

    int count = 0;
    pStream->Read( &count, 4 );
    if( count == 1 )
    {
        pStream->Read( &m_textBBox.left, 4 );
        pStream->Read( &m_textBBox.top, 4 );
        pStream->Read( &m_textBBox.right, 4 );
        pStream->Read( &m_textBBox.bottom, 4 );
    }
    else
    {
        m_textBBox.left = 0;
        m_textBBox.top = 0;
        m_textBBox.right = -1;
        m_textBBox.bottom = -1;
    }

    pStream->Read( &count, 4 );
    if( count > 0 )
    {
        m_graphBBoxes.resize( count );
        for( int i = 0; i < count; i++ )
        {
            pStream->Read( &m_graphBBoxes[i].left, 4 );
            pStream->Read( &m_graphBBoxes[i].top, 4 );
            pStream->Read( &m_graphBBoxes[i].right, 4 );
            pStream->Read( &m_graphBBoxes[i].bottom, 4 );
        }
    }
    UpdateBBox();
}
bool TShapeInfo::LoadFromStream2( TStream* pStream )
{
    pStream->Read( &m_id, 4 );

    int n = 0;
    pStream->Read( &n, 2 );
    m_guid.SetLength( n );
    pStream->Read( m_guid.c_str(), n ); // guid
    pStream->Read( &m_iconId, 4 ); //iconid

    pStream->Read( &m_type, 4 );
    m_orgType = m_type;

    if( m_type < 4 )
        m_type = 2;
    else if( m_type == 4 )
        m_type = 4;
    else if( m_type > 4 && m_type < 11 )
        m_type = 8;
    else if( m_type == 11 )
        m_type = 16;
    else if( m_type == 13 )
        m_type = 64;
    else if( m_type == 12 )
        m_type = 32;
    else if( m_type == 14 )
        m_type = 17;
    else if( m_type == 15 )
        m_type = 18;
    else if( m_type == 16 )
        m_type = 19;
    else if( m_type == 17 )
        m_type = 20;
    else if( m_type == 18 )
        m_type = 21;
    else if( m_type == 19 )
        m_type = 22;
    else if( m_type == 20 )
        m_type = 23;
    else
        m_type = 63;

    pStream->Read( &m_angle, 4 );
    pStream->Read( &x, 4 );
    pStream->Read( &y, 4 );
    pStream->Read( &width, 4 );
    pStream->Read( &height, 4 );

    pStream->Read( &m_group, 4 );

    int count = 0;
    pStream->Read( &count, 4 );
    if( count == 1 )
    {
        pStream->Read( &m_textBBox.left, 4 );
        pStream->Read( &m_textBBox.top, 4 );
        pStream->Read( &m_textBBox.right, 4 );
        pStream->Read( &m_textBBox.bottom, 4 );
    }
    else
    {
        m_textBBox.left = 0;
        m_textBBox.top = 0;
        m_textBBox.right = -1;
        m_textBBox.bottom = -1;
    }

    pStream->Read( &count, 4 );
    if( count > 0 )
    {
        m_graphBBoxes.resize( count );
        for( int i = 0; i < count; i++ )
        {
            pStream->Read( &m_graphBBoxes[i].left, 4 );
            pStream->Read( &m_graphBBoxes[i].top, 4 );
            pStream->Read( &m_graphBBoxes[i].right, 4 );
            pStream->Read( &m_graphBBoxes[i].bottom, 4 );
        }
    }
    UpdateBBox();
}
static void My_UnionRect( RECT* b, RECT* b1, RECT* b2 )
{
    RECT bt;
    bt.left = min( min( b1->left, b2->left ), min( b1->right, b2->right ) );
    bt.right = max( max( b1->left, b2->left ), max( b1->right, b2->right ) );
    bt.top = min( min( b1->top, b2->top ), min( b1->bottom, b2->bottom ) );
    bt.bottom = max( max( b1->top, b2->top ), max( b1->bottom, b2->bottom ) );

    *b = bt;
}
void TShapeInfo::UpdateBBox()
{
    m_box = m_textBBox;
    for( int i = 0; i< m_graphBBoxes.size(); i++ )
    {
        if( i == 0 && m_box.bottom < m_box.top )
        {
            //m_box = m_graphBBoxes[i];
            My_UnionRect( &m_box, &m_graphBBoxes[i], &m_graphBBoxes[i] );
        }
        else
        {
            My_UnionRect( &m_box, &m_box, &m_graphBBoxes[i] );
        }
    }

    if( m_box.Width() <= 1 )
    {
        m_box.Left --;
        m_box.right ++;
    }
    if( m_box.Height() <= 1 )
    {
        m_box.top --;
        m_box.bottom ++;
    }
    if( GetId() == 21 || GetId() == 23 )
    {
        ::InflateRect( &m_box, 3, 3 );
    }
}

static int PtToLine( int x, int y, int x1, int y1, int x2, int y2 )
{
    int SPLIT_TOLERANCE =3;

    int dx, dy;
    int f;

    dx = x1 - x2;
    dy = y1 - y2;
    if( abs(dy) < 2 ) // 'horizen line
	{
        if( x < min(x1, x2) - SPLIT_TOLERANCE || x > max(x1, x2) + SPLIT_TOLERANCE )
            f = min(sqrt((x - x1) * (x - x1) + (y - y1) * (y - y1)), sqrt((x - x2) * (x - x2) + (y - y2) * (y - y2)));
        else
            f = abs(y - y1);

        return f;
	}
    else if( abs(dx) < 2 ) //0.0001 Then ' //horizen line
	{

        if( y < min(y1, y2) - SPLIT_TOLERANCE || y > max(y1, y2) + SPLIT_TOLERANCE )
            f = min(sqrt((x - x1) * (x - x1) + (y - y1) * (y - y1)), sqrt((x - x2) * (x - x2) + (y - y2) * (y - y2)));
        else
            f = abs(x - x1);

        return f;
	}
    else
	{
        //if( x < min(x1, x2) - SPLIT_TOLERANCE || y < min(y1, y2) - SPLIT_TOLERANCE || x > max(x1, x2) + SPLIT_TOLERANCE || y > max(y1, y2) + SPLIT_TOLERANCE )
        double L;
        L = (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);
        double r;
        r = ((x - x1) * (x2 - x1) + (y - y1) * (y2 - y1)) / L;

        if( r <= 0 || r >= 1 )
            f = min(sqrt((x - x1) * (x - x1) + (y - y1) * (y - y1)), sqrt((x - x2) * (x - x2) + (y - y2) * (y - y2)));
		else
            f = abs(dy * (x - x1) - dx * (y - y1)) / sqrt(dx * dx + dy * dy);

        return f;
	}
}

static int Circle3P( double x1, double y1, double x2, double y2, double x3, double y3, double& xc, double& yc, double& fRadius)
{
        double Div;

        // Check points are distinct
        if((x1 == x2 && y1 == y2) || (x1 == x3 && y1 == y3) || (x2 == x3 && y2 == y3))
			return 1;

        // Calculate x-coordinate of centre
        Div = 2.0 * ((x1 - x2)*(y2 - y3)-(x2 - x3)*(y1 - y2));
        if(Div == 0.0)
			return 1;
        xc = ((y2 - y3) *( x1*x1 - x2*x2 + y1*y1 - y2*y2) -
            (y1 - y2) * (x2*x2 - x3*x3 + y2*y2 - y3*y3)) / Div;

        // Calculate y-coordinate
        if(y1 != y2)
			yc = ((2.0 * xc * (x2-x1)) + (x1*x1 - x2*x2 + y1*y1 - y2*y2))/
                                (2.0 * (y1-y2));
        else
			yc = ((2.0 * xc * (x3-x1)) + (x1*x1 - x3*x3 + y1*y1 - y3*y3))/
                    (2.0 * (y1-y3));

        // Derive radius
        fRadius = sqrt((xc - x1) * (xc - x1) + (yc - y1) * (yc - y1));

        // Convert results to integers
//        if(Math.Abs(xc) > (double)Int32.MaxValue) return 2;
//        if(Math.Abs(yc) > (double)Int32.MaxValue) return 2;
//        if(Math.Abs(fRadius) > (double)Int32.MaxValue) return 2;
        // Return success
        return 0;
}

int TShapeInfo::HitTest( int x, int y )
{
    if( XYInRect( &m_box, x, y ) )
    {
        if( GetIconId() == 325 )
        {
            return 0;
        }
        if( XYInRect( &m_textBBox, x, y ) )
            return 1;
        // 1D shape
        if( GetType() == 64 )//|| GetType() == 18 || GetType() == 20 )
        {
            for( int i = 0; i < m_graphBBoxes.size(); i++ )
            {
                TRect& rc = m_graphBBoxes[i];
                if( PtToLine( x, y, rc.left, rc.top, rc.right, rc.bottom  ) <= 3 * GetAccuracy() )
                    return i + 2;
            }
        }
        else if( GetType() == 21 || GetType() == 23)
        {
            if( m_graphBBoxes.size() == 1 )
            {
                TRect& rc = m_graphBBoxes[0];
                if( PtToLine( x, y, rc.left, rc.top, rc.right, rc.bottom  ) <= 3 * GetAccuracy() )
                    return 2;
            }
            else if( m_graphBBoxes.size() == 2 )
            {
                int x1 = m_graphBBoxes[0].left;
                int y1 = m_graphBBoxes[0].top;

                int x3 = m_graphBBoxes[0].right;
                int y3 = m_graphBBoxes[0].bottom;

                int x2 = m_graphBBoxes[1].right;
                int y2 = m_graphBBoxes[1].bottom;

                if( (x2 == x1 && y2 == y1) || (x2 == x3 && y2 == y3) )
                {
                    x2 = m_graphBBoxes[1].left;
                    y2 = m_graphBBoxes[1].top;
                }

                double xc, yc, radius;
                if( Circle3P( x1, y1, x2, y2 ,x3, y3, xc, yc, radius ) == 0 )
                {
                    double r = sqrt( (x-xc)*(x-xc) + (y-yc)*(y-yc) );
                    if( abs( r - radius ) <= 5 )
                    {
                        return 2;
                    }
                }
            }
            else
            {
                //?
            }
        }
        else
        {
            for( int i = 0; i < m_graphBBoxes.size(); i++ )
            {
                if( XYInRect( &m_graphBBoxes[i], x, y ) )
                    return i + 2;
            }
        }
    }
    return 0;
}
//============================================================
TSigDrawing::TSigDrawing()
{
    m_image = NULL;
    FDrawingID = -1;
}
void __fastcall TSigDrawing::OnZipError(TStream* Stream, int & ErrorCode)
{
    //Zlibex::TCustomZStream* pStream = dynamic_cast<Zlibex::TCustomZStream*>( Stream );
    ErrorCode = 0;
}

typedef struct {
  TLogPalette lPal;
  TPaletteEntry dummy[16];
} LogPal;

static HPALETTE CreatePaletteFromMetaFile (HENHMETAFILE hemf, int* count )
{
    HPALETTE        hPalette ;
    int          	iNum ;
    LOGPALETTE *	plp ;

    if (!hemf)
        return NULL ;
    iNum = GetEnhMetaFilePaletteEntries (hemf, 0, NULL);
    *count = iNum;
    if (0 == iNum)
            return NULL ;
    plp = (LOGPALETTE *) new char[sizeof (LOGPALETTE) + (iNum - 1) * sizeof (PALETTEENTRY)];
    plp->palVersion    = 0x0300 ;
    plp->palNumEntries = iNum ;

    GetEnhMetaFilePaletteEntries (hemf, iNum, plp->palPalEntry) ;
    hPalette = CreatePalette (plp) ;
    delete[] plp;
    return hPalette ;
}

bool TSigDrawing::LoadFromStream( TStream* pStream )
{
    int oldPos = pStream->Position;

    TStream* ms = pStream;

    if( !m_image )
    {
        m_image = new TImage( 0 );
        m_image->AutoSize = true;
        m_image->Center = true;
    }
    
    bool isBmp = false;
    bool isGif = false;
    char bm[2];
    ms->Read( bm, 2 );
    if( bm[0] != 'B' || bm[1] != 'M' )
    {
        isBmp = false;
        if( bm[0] == 'G' && bm[1] == 'I' )
            isGif = true;
        else
            isGif = false;
    }
    else
        isBmp = true;

//    ms->Seek( 0, soFromBeginning );
//    ms->SaveToFile( "R:\\drawing.bmp" );
    ms->Seek( -4, soFromEnd );

    int pos;
    ms->Read( &pos, 4 );

    if( !m_image )
    {
        m_image = new TImage( 0 );
        m_image->AutoSize = true;
    }
    ms->Seek( 0, soFromBeginning );
    if( isBmp )
        m_image->Picture->Bitmap->LoadFromStream( ms );
    else if( isGif )
    {
        //TGIFImage* gifImage = new TGIFImage();
        //gifImage->LoadFromStream( ms );

        //m_image->Picture->Bitmap->Assign( gifImage->Bitmap );
        //delete gifImage;
    }
    else
    {
        CMetaFile meta;
        char* buf = new char[pos];
        ms->Read( buf, pos -  1 );
        meta.Read( buf, pos - 1 );
        delete []buf;
        m_image->Picture->Bitmap = new Graphics::TBitmap;
/*
        int palSize = 0;
        HPALETTE hPal = CreatePaletteFromMetaFile( meta.GetHandle(), &palSize );
        if( hPal == 0 )
        {
            if( m_defBitmap16 )
                m_image->Picture->Bitmap->Assign( m_defBitmap16 );
        }
        else
        {
            if( palSize <= 16 )
                m_image->Picture->Bitmap->PixelFormat = pf4bit;
            else
                m_image->Picture->Bitmap->PixelFormat = pf8bit;
            m_image->Picture->Bitmap->Palette = hPal;
        }
*/        
/*
        m_image->Picture->Bitmap->PixelFormat = pf4bit;

        LogPal SysPal;
        SysPal.lPal.palVersion = 0x300;
        SysPal.lPal.palNumEntries = 16;
        HDC dc = ::GetDC( 0 );

static const COLORREF win_clr[16] = {
  PALETTERGB(0x00, 0x00, 0x00),  // BLACK 15
  PALETTERGB(0xFF, 0xFF, 0xFF),  // WHITE 0
  PALETTERGB(0xA0, 0xA0, 0xA0),  // GRAY 13
  PALETTERGB(0xFF, 0x92, 0x00),  // ORANGE 2
  PALETTERGB(0xB0, 0x00, 0x00),  // RED 3
  PALETTERGB(0x00, 0xB0, 0x00),  // GREEN 9
  PALETTERGB(0x00, 0x00, 0xFF),  // BLUE 6
  PALETTERGB(0xC8, 0x64, 0x00),  // UMBER 10
  PALETTERGB(0x70, 0x70, 0x70),  // DARKGRAY 14
  PALETTERGB(0xD0, 0xD0, 0xD0),  // LIGHTGRAY 12
  PALETTERGB(0xA5, 0x00, 0xFF),  // VIOLET 5
  PALETTERGB(0xFF, 0xFD, 0x00),  // YELLOW 1
  PALETTERGB(0xFF, 0x00, 0xBC),  // LIGHTRED 4
  PALETTERGB(0x00, 0xFF, 0x00),  // LIGHTGREEN 8
  PALETTERGB(0x00, 0xC8, 0xFF),  // LIGHTBLUE 7
  PALETTERGB(0xFF, 0xCC, 0x80)   // LIGHTUMBER 11 
};        
        for (int i = 0; i < 16; i++)
        {
            SysPal.lPal.palPalEntry[i].peRed = GetRValue(win_clr[i]);
            SysPal.lPal.palPalEntry[i].peGreen = GetGValue(win_clr[i]);
            SysPal.lPal.palPalEntry[i].peBlue = GetBValue(win_clr[i]);
            SysPal.lPal.palPalEntry[i].peFlags = PC_NOCOLLAPSE;
        }
        HPALETTE hPal;
        if (!(hPal = CreatePalette((LPLOGPALETTE) &SysPal.lPal)))
            ;//quit("Cannot create palette");
        m_image->Picture->Bitmap->Palette = hPal;
        ::ReleaseDC( 0, dc );
*/

        m_image->Picture->Bitmap->Width = meta.GetWidth() * 144 / 96;
        m_image->Picture->Bitmap->Height = meta.GetHeight() * 144 / 96;

//        m_image->Picture->Bitmap->Width = meta.GetWidth() * 144 / meta.GetInch() * 0.951;
//        m_image->Picture->Bitmap->Height = meta.GetHeight() * 144 / meta.GetInch() * 0.951;

        m_image->Picture->Bitmap->Canvas->Brush->Color = clWhite;
        m_image->Picture->Bitmap->Canvas->Rectangle( 0, 0, m_image->Picture->Bitmap->Width, m_image->Picture->Bitmap->Height );
//        meta.Display( m_image->Picture->Bitmap->Canvas->Handle,
//                    TRect( 0, 0, m_image->Width, m_image->Height)
//                    );
        meta.PlayEnhMetaFile( m_image->Picture->Bitmap->Canvas->Handle,
                    TRect( 0, 0, m_image->Width, m_image->Height)
                    );
    }
    
//    m_image->Picture->Bitmap->LoadFromStream( ms );

//    ms->Seek( 0, soFromBeginning );
//    ms->SaveToFile( "R:\\drawing.bmp" );
    ms->Seek( -4, soFromEnd );

    ms->Read( &pos, 4 );
    ms->Seek( pos - 1, soFromBeginning );
//    m_drawingInfo.LoadFromStream( ms );

    if( isBmp )
        GraphType = gtBMP;
    else
        GraphType = gtEMF;

    bool result = false;

    char header[2];
    ms->Read( header, 2 );
    if( header[0] == 'B' && header[1] == 'S' )
    {
        int ver = 0;
        ms->Read( &ver, 4 );
        if( ver == 0xFFFF0000 + 1 )
        {
            ms->Read( &FDrawingID, 4 );

            ms->Read( &m_box.left, 4 );
            ms->Read( &m_box.top, 4 );
            ms->Read( &m_box.right, 4 );
            ms->Read( &m_box.bottom, 4 );

            int count = 0;
            ms->Read( &count, 4 );

            m_shapes.resize( count );

            for( int i = 0; i < count; i++ )
                m_shapes[i].LoadFromStream( ms );

            result = true;
        }
    }
    else if( header[0] == 'B' && header[1] == 'N' )
    {
        int ver = 0;
        ms->Read( &ver, 4 );
        if( ver == 0xFFFF0000 + 1 || ver == 0xFFFF0000 + 2)
        {
            ms->Read( &FDrawingID, 4 );

            ms->Read( &m_box.left, 4 );
            ms->Read( &m_box.top, 4 );
            ms->Read( &m_box.right, 4 );
            ms->Read( &m_box.bottom, 4 );

            int count = 0;
            ms->Read( &count, 4 );

            m_shapes.resize( count );

            for( int i = 0; i < count; i++ )
                m_shapes[i].LoadFromStream2( ms );

            result = true;
        }
    }

    return result;
}
bool TSigDrawing::LoadFromCompressedStream( TStream* pStream )
{
    int oldPos = pStream->Position;
    char gif[3];
    pStream->Read( gif, 3 );
    pStream->Seek( oldPos, soFromBeginning );

    TMemoryStream* ms = new TMemoryStream();
    if( gif[0] == 'G' && gif[1] == 'I' && gif[2] == 'F' )
    {
        ms->CopyFrom( pStream, 0 );
    }
    else
    {
/*
        TZDecompressionStream* ds = new TZDecompressionStream( pStream );
        ds->OnProgress = 0;
    //    ds->OnError = OnZipError;
    //    ms->CopyFrom( ds, 0 );
        char buf[65536];
        while( int count = ds->Read(buf, sizeof(buf)) )
        {
            ms->Write( buf, count );
        }
        delete ds;
*/
    }
    ms->Seek( 0, soFromBeginning );

    bool result = LoadFromStream( ms );

    delete ms;

    return result;
}

const TShapeInfo* TSigDrawing::HitTest( int x, int y )
{
    for( int i = m_shapes.size() -1; i >= 0; i-- )
    {

        if( m_shapes[i].HitTest( x, y ) && m_shapes[i].GetIconId() != 5107)
        {
            return &m_shapes[i];
        }
    }
    return 0;
}

void __fastcall TSigDrawing::SetType(int value)
{
    if(FType != value) {
        FType = value;
    }
}
int __fastcall TSigDrawing::GetType()
{
    return FType;
}

void __fastcall TSigDrawing::SetGraphType(emGraphType value)
{
    if(FGraphType != value) {
        FGraphType = value;
    }
}
TSigDrawing::emGraphType __fastcall TSigDrawing::GetGraphType()
{
    return FGraphType;
}

bool TSigDrawing::SaveToBitmap(AnsiString bmpFile)
{
    TFileStream* file = new TFileStream( bmpFile, fmCreate );
    m_image->Picture->Bitmap->SaveToStream( file );
    delete file;
}

bool TSigDrawing::SaveToGIF(AnsiString gifFile)
{
/*
    TGIFImage* gif = new TGIFImage();
    gif->Assign( m_image->Picture->Bitmap );
    gif->SaveToFile( gifFile );
    delete gif;
*/
}

AnsiString TSigDrawing::EnumShapes( std::vector<long>& idAll, long* Index )
{
    int pos = max( 0L, *Index );
    // 效率
    int idCount = idAll.size();
    if( idCount == 0 )
    {
        if( pos < m_shapes.size() )
        {
            for( int i = pos; i < m_shapes.size(); i++ )
            {
                if( m_shapes[i].GetIconId() > 0 )
                {
                    *Index = i;
                    return m_shapes[i].GetGuid() + "_" + AnsiString( m_shapes[i].GetGroup() );
                }
            }
        }
    }
    else
    {
        for( int i = pos; i < m_shapes.size(); i++ )
        {
            int iconId = m_shapes[i].GetIconId();
            for( int m = 0; m < idCount; m++ )
            {
                if( iconId == idAll[m] )
                {
                    *Index = i;
                    return m_shapes[i].GetGuid() + "_" + AnsiString( m_shapes[i].GetGroup() );
                }
            }
        }
    }
    *Index = -1;
    return "";
}
