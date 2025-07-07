namespace ASTRedux.Data.RSound.Sub;

internal struct CSRHead
{
    public const int CSR_MAGIC = 0x20525343; // csr (CSoundRequest?)
    public int CSRDataSize { get; set; } // size of csr block

    public int CSREntryCount { get; set; } // entry = 32

    public int Unknown_1 { get; set; } // noted values: 0x04 and 0x06

    public int CSRDataStart { get; set; } // relative offset to csr entries

    public int Unknown_2 { get; set; }

    public int CSRUnknownEntriesOffset { get; set; } // points to 0x20 long entries with strange data and lots of padding
}
