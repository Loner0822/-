// metaex.cpp - Code for CMetaFile
#include "meta.h"

CMetaFile::CMetaFile()
{
	m_emf = NULL;
	::ZeroMemory(&m_aldusMFHeader, sizeof(m_aldusMFHeader));
}

CMetaFile::~CMetaFile()
{
	if (m_emf)
		::DeleteEnhMetaFile(m_emf);
}

static unsigned int IsEMF1(const unsigned char *magick,const size_t length)
{
    if (length < 48)
        return false;
    if (memcmp(magick+40,"\040\105\115\106\000\000\001\000",8) == 0)
        return true;
    return false;
}
static bool IsEMF( const unsigned char *magick,const size_t length )
{
    ENHMETAHEADER header;
    if( length > sizeof(header) )
    {
        ENHMETAHEADER* p = (ENHMETAHEADER*)magick;
        return p->iType == EMR_HEADER && p->dSignature == ENHMETA_SIGNATURE;
    }
    else
    {
        return false;
    }
}

bool CMetaFile::Read(char* buffer, int length)
{
	DWORD   		dwIsAldus;
	METAHEADER		mfHeader;
  	DWORD    		dwSize;
	DWORD			seekpos;

    if( IsEMF( buffer, length ) )
    {
        ENHMETAHEADER* p = (ENHMETAHEADER*)buffer;
        if( length >= p->nBytes )
            m_emf = ::SetEnhMetaFileBits( p->nBytes, buffer );

        if( m_emf )
        {
            m_aldusMFHeader.bbox.left = p->rclBounds.left;
            m_aldusMFHeader.bbox.top = p->rclBounds.top;
            m_aldusMFHeader.bbox.right = p->rclBounds.right;
            m_aldusMFHeader.bbox.bottom = p->rclBounds.bottom;

            m_aldusMFHeader.inch = ( p->rclFrame.right - p->rclFrame.left ) / ( p->rclBounds.right - p->rclBounds.left );
            m_inch = ( p->rclBounds.right - p->rclBounds.left ) / (float)( p->rclFrame.right - p->rclFrame.left ) * 25.4 * 100;
        }

        return m_emf ? true : false;
    }

    if( length >=0 && length < sizeof(dwIsAldus) )
		return false;
	::memcpy(&dwIsAldus, buffer, sizeof(dwIsAldus));

	if (dwIsAldus != ALDUSKEY)
	{
		// A windows metafile, not a placeable wmf
		seekpos = 0;
		::ZeroMemory(&m_aldusMFHeader, sizeof(m_aldusMFHeader));
	}
	else
	{
		// This is a placeable metafile
        // Convert the placeable format into something that can
        // be used with GDI metafile functions
        if( length >= 0 && length < ALDUSMFHEADERSIZE )
			return false;
		memcpy( &m_aldusMFHeader, buffer, ALDUSMFHEADERSIZE );
		seekpos = ALDUSMFHEADERSIZE;
	}
	// Read the metafile header
    if( length >= 0 && length < sizeof(mfHeader) )
		return false;
	memcpy(&mfHeader, buffer + seekpos, sizeof(mfHeader) );
	// At this point we have a metafile header regardless of whether
	// the metafile was a windows metafile or a placeable metafile
	// so check to see if it is valid.  There is really no good
	// way to do this so just make sure that the mtType is either
	// 1 or 2 (memory or disk file)
	if ((mfHeader.mtType != 1) && (mfHeader.mtType != 2))
		return false;
	// Allocate memory for the metafile bits
	dwSize = mfHeader.mtSize * 2;
	// Read metafile bits and create
	// the enhanced metafile
    if( length >=0 && length < dwSize )
        return false;

    if( m_aldusMFHeader.inch <= 0 )
        m_aldusMFHeader.inch = 96;
    METAFILEPICT  mp;
    mp.mm=MM_ANISOTROPIC;
    mp.xExt=m_aldusMFHeader.bbox.right-m_aldusMFHeader.bbox.left;
    mp.xExt=(mp.xExt*2540l)/(DWORD) (m_aldusMFHeader.inch);
    mp.yExt=m_aldusMFHeader.bbox.bottom-m_aldusMFHeader.bbox.top;
    mp.yExt=(mp.yExt*2540l)/(DWORD) (m_aldusMFHeader.inch);
    mp.hMF=NULL;

    HDC hDC=GetDC(NULL);
    m_emf = ::SetWinMetaFileBits(dwSize, buffer + seekpos, hDC, &mp );
    ReleaseDC(NULL,hDC);

    m_inch = m_aldusMFHeader.inch;
    
	return m_emf ? true : false;
}
bool CMetaFile::Read(FILE* file)
{
    int pos = ftell( file );
    fseek( file, 0L, SEEK_END);
    int fileLen = ftell( file );
    fseek( file, pos, SEEK_SET );

    char * buf = new char[ fileLen ];
    fread( buf, 1, fileLen, file );
    Read( buf );
    delete []buf;

	return m_emf ? true : false;
}

bool CMetaFile::ReadFile(LPCTSTR lpszFileName)
{
	try
	{
		// Open the file
		FILE* file = fopen( lpszFileName, "rb" );
        if( file )
        {
    		Read(file);
            fclose( file );
        }
	}
	catch(...)
	{
        return false;
	}
	return true;
}

void CMetaFile::Display(HDC dc, RECT& r, METAALIGNMENT align)
{
	// Check for valid enhanced metafile
	if (!m_emf)
		return;
	// Set the bounding rectangle
	// Thios may be changed by the
	// alignment code to preserve
	// aspect ratio, etc.
	RECT rectBounds = r;
	// Set alignment
	// We can only do this if we can
	// get the size of the metafile
	// This should be possible for
	// metafiles with Aldus headers
	if (m_aldusMFHeader.inch)
	{
		// Get the logical size of the metafile
		SIZE size;
		size.cx = 254 * (m_aldusMFHeader.bbox.right - m_aldusMFHeader.bbox.left) / m_aldusMFHeader.inch;
		size.cy = 254 * (m_aldusMFHeader.bbox.bottom - m_aldusMFHeader.bbox.top) / m_aldusMFHeader.inch;
		// We now have the logical size
		// of the metafile.  We can use
		// this to create a rectangle
		// in which to display it - with
		// the correct alignment
		FitPicture(rectBounds, size, align);
	}
	// Play metafile
	::PlayEnhMetaFile( dc, m_emf, &rectBounds );
}
bool CMetaFile::PlayEnhMetaFile( HDC dc, RECT& r )
{
	if (!m_emf)
		return false;
    return ::PlayEnhMetaFile( dc, m_emf, &r );
}

// Fit the metafile by altering the rectangle
// passed accordingly.  Returns true if the
// picture needs stretching
void CMetaFile::FitPicture(RECT& r, SIZE& size, METAALIGNMENT align)
{
	RECT rectPrev = r;

	// First, sort out the height
	// of the rectangle
	switch (align)
	{
	case AlignTopLeft:
	case AlignTopCentre:
	case AlignTopRight:
		r.bottom = r.top + size.cy;
		break;
	case AlignMiddleLeft:
	case AlignMiddleCentre:
	case AlignMiddleRight:
		r.top = (r.bottom + r.top - size.cy) / 2;
		r.bottom = r.top + size.cy;
		break;
	case AlignBottomLeft:
	case AlignBottomCentre:
	case AlignBottomRight:
		r.top = r.bottom - size.cy;
		break;
	case AlignFit:
		// Make as large as possible, but keep aspect ratio.
		if (((r.right - r.left) * size.cy) > ((r.bottom - r.top) * size.cx))
		{
			// Picture is taller.
			// Adjust left and right and leave
			// top and bottom alone.
			int w = (size.cx * (r.bottom - r.top)) / size.cy;
			r.left = (r.left + r.right - w) / 2;
			r.right = r.left + w;
		}
		else
		{
			int h = (size.cy * (r.right - r.left)) / size.cx;
			r.top = (r.top + r.bottom - h) / 2;
			r.bottom = r.top + h;
		}
		return;
	case AlignStretch:
		return;
	}

	// Sort out the rectangle width
	switch (align)
	{
	case AlignTopLeft:
	case AlignMiddleLeft:
	case AlignBottomLeft:
		r.right = r.left + size.cx;
		break;
	case AlignTopCentre:
	case AlignMiddleCentre:
	case AlignBottomCentre:
		r.left = (r.right + r.left - size.cx) / 2;
		r.right = r.left + size.cx;
		break;
	case AlignTopRight:
	case AlignMiddleRight:
	case AlignBottomRight:
		r.left = r.right - size.cx;
		break;
	}
	if (rectPrev.left > r.left)
		r.left = rectPrev.left;
	if (rectPrev.right < r.right)
		r.right = rectPrev.right;
	if (rectPrev.top > r.top)
		r.top = rectPrev.top;
	if (rectPrev.bottom < r.bottom)
		r.bottom = rectPrev.bottom;
	return;
}

/*
void CMetaFile::Serialize(CArchive& archive)
{
	if (archive.IsStoring())
	{
		// Save
		// Archive the header
		archive.Write(&m_aldusMFHeader, sizeof(m_aldusMFHeader));
		// We need to save the raw
		// bits of the metafile
		// We should save the size
		// of these bits first
		UINT nSize = 0;
		if (m_emf)
		{
			// Get size of metafile
			nSize = ::GetEnhMetaFileBits(m_emf, 0, NULL);
			// Now get the bits and save
			BYTE* pBits = new BYTE [nSize];
			if (pBits)
			{
				// Get metafile bits
				::GetEnhMetaFileBits(m_emf, nSize, pBits);
				// Write to archive
				archive << nSize;
				archive.Write(pBits, nSize);
				// Free memory
				delete [] pBits;
			}
			else
			{
				nSize = 0;
				archive << nSize;
			}
		}
		else
		{
			// Empty
			archive << nSize;
		}
	}
	else
	{
		// Load
		// Get the header
		archive.Read(&m_aldusMFHeader, sizeof(m_aldusMFHeader));
		// Get the metafile bits size
		UINT nSize;
		archive >> nSize;
		if (nSize)
		{
			// Allocate a buffer
			//BYTE* pBits = (BYTE *)GlobalAllocPtr(GHND, nSize);
			BYTE* pBits = new BYTE [nSize];
			if (pBits)
			{
				// Read bits
				archive.Read(pBits, nSize);
				// Set metafile
				m_emf = ::SetEnhMetaFileBits(nSize, pBits);
			}
			else
			{
				// Allocation error
				// Seek past bit data
				CFile* pFile = archive.GetFile();
				if (pFile)
					pFile->Seek(nSize, CFile::current);
			}
		}
	}
}
*/
void CMetaFile::Clear()
{
	if (m_emf)
	{
		::DeleteEnhMetaFile(m_emf);
		m_emf = NULL;
		::ZeroMemory(&m_aldusMFHeader, sizeof(m_aldusMFHeader));
	}
}

bool CMetaFile::IsEmpty()
{
	return !m_emf;
}

/*
bool CMetaFile::IsMetafile(FILE* pFile)
{
	pFile->Seek(0, CFile::begin);
	DWORD dwIsAldus;
	DWORD dwSeekPos = 0;
	if (pFile->Read(&dwIsAldus, sizeof(dwIsAldus)) != sizeof(dwIsAldus))
		return false;
	if (dwIsAldus == ALDUSKEY)
	{
		// This is a placeable metafile
	        // Convert the placeable format into something that can
	        // be used with GDI metafile functions 
		pFile->Seek(0, CFile::begin);
		ALDUSMFHEADER aldusMFHeader;
		if (pFile->Read(&aldusMFHeader, ALDUSMFHEADERSIZE) != ALDUSMFHEADERSIZE)
			return false;
		dwSeekPos = ALDUSMFHEADERSIZE;
	}
	pFile->Seek(dwSeekPos, CFile::begin);
	METAHEADER mfHeader;
	//read the metafile header
	if (pFile->Read(&mfHeader, sizeof(mfHeader)) != sizeof(mfHeader))
		return false;
	// Check header bytes
	return (mfHeader.mtType == 1 || mfHeader.mtType == 2);
}
*/
int CMetaFile::GetWidth() const
{
	return abs(m_aldusMFHeader.bbox.right - m_aldusMFHeader.bbox.left + 1);
}

int CMetaFile::GetHeight() const
{
	return abs(m_aldusMFHeader.bbox.bottom - m_aldusMFHeader.bbox.top + 1);
}


