using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int row;         // Hücrenin satýr konumu
    public int column;      // Hücrenin sütun konumu
    public Color cellColor; // Hücrenin rengi

    private GridManager gridManager;       // GridManager referansý

    public CellType cellType; // hücrenin tipini tutucak

    // Hücreye ait verileri baþlatan fonksiyon
    public void Init(int row, int column, Color color, GridManager manager)
    {
        this.row = row;              // Satýr konumunu ata
        this.column = column;        // Sütun konumunu ata
        this.cellColor = color;      // Hücre rengini ata
        this.gridManager = manager;  // GridManager referansýný ata
    }

    // Hücreye týklandýðýnda tetiklenen olay
    private void OnMouseDown()
    {
        if (gridManager != null)
        {
            // Týklanan hücre bilgilerini GridManager'a gönder
            gridManager.OnCellClicked(row, column, cellColor, cellType);
        }
    }

    public enum CellType
    {
        Normal,        // Normal hücre (standart davranýþ)
        Explosive,     // Patlayýcý hücre
        Joker,         // Joker hücre
        ColorChanger   // Renk deðiþtirici hücre
    }


}
