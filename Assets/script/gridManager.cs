// GridManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridManager : MonoBehaviour
{
    public int rows = 5;
    public int columns = 5;
    public GameObject cellPrefab;
    public Vector2 cellSize = new Vector2(1, 1);
    public Color[] possibleColors;
    public Color[] specialsColor;

    private Cell[,] grid;
    private ScoreManager scoreManager;

    private void Start()
    {
        grid = new Cell[rows, columns];
        scoreManager = GetComponent<ScoreManager>();
        GenerateGrid();
    }

    // Grid olu�turan fonksiyon
    private void GenerateGrid()
    {

        // a��klama;
        // �ncelikle sat�r ve s�tun say�lar�n� al�yor. sonras�nda ise sat�r ve s�tun 0 dan ba�lad��� i�in 1 ��kart�yor.
        // Sonras�nda ise h�cre boyutu ile �arp�p s�tunda ve sat�rda ne kadar �telenmesi gerekti�ini buluyor.
        Vector2 startPos = new Vector2(-((columns - 1) * cellSize.x) / 2, -((rows - 1) * cellSize.y) / 2);

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Vector2 spawnPosition = new Vector2(column * cellSize.x, row * cellSize.y) + startPos;
                GameObject newCellObj = Instantiate(cellPrefab, spawnPosition, Quaternion.identity, transform);
                newCellObj.name = $"Cell_{row}_{column}";

                if (newCellObj.GetComponent<BoxCollider2D>() == null)
                {
                    newCellObj.AddComponent<BoxCollider2D>();
                }

                Cell cellScript = newCellObj.AddComponent<Cell>();


                SpriteRenderer sr = newCellObj.GetComponent<SpriteRenderer>();
                if (sr == null)
                {
                    sr = newCellObj.AddComponent<SpriteRenderer>();
                }


                //H�cre tipi belirleme
                int randomSayi = Random.Range(0, 100);
                // special color s�ras�yla siyah beyaz gri
                if (randomSayi < 10) // y�zde 10 ihtimalle
                {
                    cellScript.cellType = Cell.CellType.Explosive;
                    Color SpecialColor = specialsColor[0];
                    sr.color = SpecialColor;

                }
                else if (randomSayi < 20)// y�zde 20 ihtimalle
                {
                    cellScript.cellType = Cell.CellType.Joker;
                    Color SpecialColor = specialsColor[1];
                    sr.color = SpecialColor;
                }
                else if (randomSayi < 30)// y�zde 30 ihtimalle
                {
                    cellScript.cellType = Cell.CellType.ColorChanger;
                    Color SpecialColor = specialsColor[2];
                    sr.color = SpecialColor;
                }
                else // y�zde 70 ihtimalle
                {
                    cellScript.cellType = Cell.CellType.Normal;
                    Color NormalColor = possibleColors[Random.Range(0, possibleColors.Length)];
                    sr.color = new Color(NormalColor.r, NormalColor.g, NormalColor.b, 1f);
                }
                //H�cre tipi belirleme
     

               

                // olu�turulan h�creye ait verileri  ilgili alana g�nderip sakl�yoruz.
                cellScript.Init(row, column, sr.color, this);

                // Grid'e h�cre ekleniyor
                // saklanan verilere ait h�creleri her d�ng�de gride ekliyoruz.
                grid[row, column] = cellScript;


            }
        }
    }

    // cell scriptinden h�creye t�klan�ld���nda, h�crenin bilgileri buraya gelicek.
    public void OnCellClicked(int row, int column, Color cellColor, Cell.CellType cellType)
    {
        List<Cell> matchingCells;

        switch (cellType)
        {
            case Cell.CellType.Normal:
                // ilk �nce mouse ile t�klan�lan h�cre bilgileri al�n�r. ilgili fonksiyona parametre olarak g�nderilir.
                // ve listeye eklenir.
                matchingCells = FindMatchingCells(row, column, cellColor);

                handleNormalCell(matchingCells);

                break;
            case Cell.CellType.Explosive:
                HandleExplosiveCell(row, column);
                break;
            case Cell.CellType.Joker:
                break;
            case Cell.CellType.ColorChanger:
                HandleColorChangerCell(row, column);
                break;
            default:
                break;
        }

        

        

    }

    // gelen �zel h�creye g�re i�lemler

    // normal h�cre fonksiyonu
    private void handleNormalCell(List<Cell> matchingCells)
    {
        // E�er zincirdeki h�cre say�s� yeterince b�y�kse (�rne�in 2'den fazla)
        if (matchingCells.Count > 1)
        {
            scoreManager.AddScore(matchingCells.Count);
            foreach (Cell cell in matchingCells)
            {
                // H�creleri yok et (�rne�in grid dizisinden sil ve ekrandan kald�r)
                grid[cell.row, cell.column] = null;
                Destroy(cell.gameObject);
            }

            // H�creleri a�a�� d���rme
            DropCells();
        }

        StartCoroutine(RefillGrid()); // Bo� kalan yerleri doldur
    }

    // patlama h�cresi fonksiyonu
    private void HandleExplosiveCell(int row, int column)
    {
        // 1 x 1 bas�ld� 0 x 0 dan ba�lad� ve 2 x 2 ye kadar d�ng�ye al�p listeye ekledi.
        // 0 x 0, 0 x 1, 0 x 2 column
        // 0 x 0, 1 x 0, 2 x 0
        List<Cell> affectedCells = new List<Cell>();

        // �evredeki 3x3 b�lgeyi tar�yoruz
        for (int r = row - 1; r <= row + 1; r++)
        {
            for (int c = column - 1; c <= column + 1; c++)
            {
                
                if (r >= 0 && r < rows && c >= 0 && c < columns && grid[r, c] != null)
                {
                    affectedCells.Add(grid[r, c]);
                    Debug.Log(grid[r, c]);
                    
                }
            }
        }

        foreach (Cell cell in affectedCells)
        {
            grid[cell.row, cell.column] = null;
            Destroy(cell.gameObject);
        }
        DropCells();
        StartCoroutine(RefillGrid());
    }

    // joker h�cresi fonksiyonu

    // renk de�i�tirme h�cresi fonksiyonu
    private void HandleColorChangerCell(int row, int column)
    {
        Cell cell = grid[row, column];
        Color randomColor = possibleColors[Random.Range(0,possibleColors.Length)];
        cell.cellColor = randomColor;
        cell.GetComponent<SpriteRenderer>().color = randomColor;

    }

    // gelen �zel h�creye g�re i�lemler


    // H�crenin etraf�ndaki ayn� renkli h�creleri tespit etme
    // OnCellClicked fonksiyonundan ba�lang�� h�cre bilgileri buraya gelir.
    private List<Cell> FindMatchingCells(int startRow, int startColumn, Color targetColor)
    {
        // ayn� renkteki h�crelerin listesi
        List<Cell> matchingCells = new List<Cell>();
        // ziyaret edilen h�crelerin k�mesi
        HashSet<Cell> visited = new HashSet<Cell>();

        // parametre olarak gelen ba�lang�� h�cre bilgileri ve liste - k�me �zyinelemeli fonksiyona gider.
        FloodFill(startRow, startColumn, targetColor, matchingCells, visited);

        return matchingCells;
    }

    // FindMatchingCells fonksiyonundan buraya parametreler gelicek
    private void FloodFill(int row, int column, Color targetColor, List<Cell> matchingCells, HashSet<Cell> visited)
    {
        // Grid s�n�rlar�n�n d���nda m�y�z?
        if (row < 0 || row >= rows || column < 0 || column >= columns)
            return;

        // H�cre var m� ve daha �nce kontrol edildi mi?
        Cell cell = grid[row, column]; // i�lem yap�lan h�creyi ifade eder.
        if (cell == null || visited.Contains(cell)) // h�cre null'sa ya da visited yani ziyaret edilen k�me i�inde
                                                    // varsa bu ad�m� atlar ve devam eder.
            return;

        // H�crenin rengi e�le�iyor mu?
        // e�le�miyorsa bu ad�m� atla ve devam et
        if (cell.cellColor != targetColor)
            return;

        // H�creyi e�le�enler listesine ekle ve kontrol edilmi� olarak i�aretle
        // h�cre kontrolleri atlat�p buraya kadar geldiyse listeye ve k�meye ekle.
        matchingCells.Add(cell);
        visited.Add(cell);

        // D�rt y�nl� kontrol (�st, alt, sa�, sol)
        FloodFill(row + 1, column, targetColor, matchingCells, visited);
        FloodFill(row - 1, column, targetColor, matchingCells, visited);
        FloodFill(row, column + 1, targetColor, matchingCells, visited);
        FloodFill(row, column - 1, targetColor, matchingCells, visited);

        // �apraz y�n kontrol�
        FloodFill(row + 1, column + 1, targetColor, matchingCells, visited);
        FloodFill(row + 1, column - 1, targetColor, matchingCells, visited);
        FloodFill(row - 1, column + 1, targetColor, matchingCells, visited);
        FloodFill(row - 1, column - 1, targetColor, matchingCells, visited);
    }

    private void DropCells()
    {
        for (int column = 0; column < columns; column++)
        {
            int emptyRow = -1; // Bo� bir sat�r bulmak i�in
            for (int row = 0; row < rows; row++)
            {
                if (grid[row, column] == null)
                {
                    if (emptyRow == -1) emptyRow = row; // �lk bo� sat�r� belirle
                }
                else if (emptyRow != -1) // Bo� sat�r bulunduktan sonra dolu h�creyi kayd�r
                {
                    // H�creyi a�a�� ta��
                    StartCoroutine(MoveCellDown(grid[row, column], emptyRow));
                    grid[emptyRow, column] = grid[row, column];
                    grid[row, column] = null;
                    grid[emptyRow, column].row = emptyRow; // Yeni pozisyonu g�ncelle
                    emptyRow++; // Bir sonraki bo� sat�ra ge�
                }
            }
        }
    }

    // H�creyi yava��a d���rmek i�in Coroutine
    private IEnumerator MoveCellDown(Cell cell, int targetRow)
    {
        Vector2 startPosition = cell.transform.position; // Ba�lang�� pozisyonu
        Vector2 endPosition = new Vector2(cell.column * cellSize.x, targetRow * cellSize.y) - new Vector2((columns - 1) * cellSize.x / 2, (rows - 1) * cellSize.y / 2); // Hedef pozisyon

        float duration = 0.5f; // H�cre d���� s�resi (yar�m saniye)
        float elapsedTime = 0f; // Ge�en zaman

        while (elapsedTime < duration)
        {
            // Pozisyonu yava��a g�ncelle
            cell.transform.position = Vector2.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime; // Zaman� art�r
            yield return null; // Bir frame bekle
        }

        cell.transform.position = endPosition; // Hedef pozisyona tam olarak yerle�tir
    }

    private IEnumerator RefillGrid()
    {
        yield return new WaitForSeconds(1f); // Bekleme s�resi, oyun ak���na g�re d�zenlenebilir

        // Yeni h�crelerin grid'e eklenmesi
        for (int column = 0; column < columns; column++)
        {
            for (int row = 0; row < rows; row++)
            {
                if (grid[row, column] == null) // E�er grid h�cresi bo�sa
                {
                    // Pozisyon hesaplamas�: Grid'in ortas�na g�re hizal�
                    Vector2 spawnPosition = GetSpawnPosition(row, column);

                    // Yeni h�cre olu�turma ve konumland�rma
                    GameObject newCellObj = CreateCell(spawnPosition, row, column);

                    // Yeni h�creyi grid'e ekle
                    grid[row, column] = newCellObj.GetComponent<Cell>();
                }
            }
        }
    }

    // H�cre olu�turma i�lemini mod�ler hale getirdik
    private GameObject CreateCell(Vector2 position, int row, int column)
    {
        GameObject newCellObj = Instantiate(cellPrefab, position, Quaternion.identity, transform);
        newCellObj.name = $"Cell_{row}_{column}";

        // H�cre i�in bile�en kontrolleri ve eklemeleri
        if (newCellObj.GetComponent<BoxCollider2D>() == null)
        {
            newCellObj.AddComponent<BoxCollider2D>();
        }

        Cell cellScript = newCellObj.AddComponent<Cell>();
        SpriteRenderer sr = newCellObj.GetComponent<SpriteRenderer>() ?? newCellObj.AddComponent<SpriteRenderer>();

        // Rastgele renk se�imi ve h�creye atanmas�
        Color randomColor = possibleColors[Random.Range(0, possibleColors.Length)];
        sr.color = new Color(randomColor.r, randomColor.g, randomColor.b, 1f);

        // H�creyi ba�lat
        cellScript.Init(row, column, sr.color, this);

        return newCellObj;
    }

    // Grid'e g�re spawn pozisyonunu hesaplayan metod
    private Vector2 GetSpawnPosition(int row, int column)
    {
        return new Vector2(
            column * cellSize.x - (columns - 1) * cellSize.x / 2,
            row * cellSize.y - (rows - 1) * cellSize.y / 2
        );
    }
}
