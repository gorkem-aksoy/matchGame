using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int row;         // H�crenin sat�r konumu
    public int column;      // H�crenin s�tun konumu
    public Color cellColor; // H�crenin rengi

    private GridManager gridManager;       // GridManager referans�

    public CellType cellType; // h�crenin tipini tutucak

    // H�creye ait verileri ba�latan fonksiyon
    public void Init(int row, int column, Color color, GridManager manager)
    {
        this.row = row;              // Sat�r konumunu ata
        this.column = column;        // S�tun konumunu ata
        this.cellColor = color;      // H�cre rengini ata
        this.gridManager = manager;  // GridManager referans�n� ata
    }

    // H�creye t�kland���nda tetiklenen olay
    private void OnMouseDown()
    {
        if (gridManager != null)
        {
            // T�klanan h�cre bilgilerini GridManager'a g�nder
            gridManager.OnCellClicked(row, column, cellColor, cellType);
        }
    }

    public enum CellType
    {
        Normal,        // Normal h�cre (standart davran��)
        Explosive,     // Patlay�c� h�cre
        Joker,         // Joker h�cre
        ColorChanger   // Renk de�i�tirici h�cre
    }


}
