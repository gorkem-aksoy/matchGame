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

    // Grid oluþturan fonksiyon
    private void GenerateGrid()
    {

        // açýklama;
        // öncelikle satýr ve sütun sayýlarýný alýyor. sonrasýnda ise satýr ve sütun 0 dan baþladýðý için 1 çýkartýyor.
        // Sonrasýnda ise hücre boyutu ile çarpýp sütunda ve satýrda ne kadar ötelenmesi gerektiðini buluyor.
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


                //Hücre tipi belirleme
                int randomSayi = Random.Range(0, 100);
                // special color sýrasýyla siyah beyaz gri
                if (randomSayi < 10) // yüzde 10 ihtimalle
                {
                    cellScript.cellType = Cell.CellType.Explosive;
                    Color SpecialColor = specialsColor[0];
                    sr.color = SpecialColor;

                }
                else if (randomSayi < 20)// yüzde 20 ihtimalle
                {
                    cellScript.cellType = Cell.CellType.Joker;
                    Color SpecialColor = specialsColor[1];
                    sr.color = SpecialColor;
                }
                else if (randomSayi < 30)// yüzde 30 ihtimalle
                {
                    cellScript.cellType = Cell.CellType.ColorChanger;
                    Color SpecialColor = specialsColor[2];
                    sr.color = SpecialColor;
                }
                else // yüzde 70 ihtimalle
                {
                    cellScript.cellType = Cell.CellType.Normal;
                    Color NormalColor = possibleColors[Random.Range(0, possibleColors.Length)];
                    sr.color = new Color(NormalColor.r, NormalColor.g, NormalColor.b, 1f);
                }
                //Hücre tipi belirleme
     

               

                // oluþturulan hücreye ait verileri  ilgili alana gönderip saklýyoruz.
                cellScript.Init(row, column, sr.color, this);

                // Grid'e hücre ekleniyor
                // saklanan verilere ait hücreleri her döngüde gride ekliyoruz.
                grid[row, column] = cellScript;


            }
        }
    }

    // cell scriptinden hücreye týklanýldýðýnda, hücrenin bilgileri buraya gelicek.
    public void OnCellClicked(int row, int column, Color cellColor, Cell.CellType cellType)
    {
        List<Cell> matchingCells;

        switch (cellType)
        {
            case Cell.CellType.Normal:
                // ilk önce mouse ile týklanýlan hücre bilgileri alýnýr. ilgili fonksiyona parametre olarak gönderilir.
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

    // gelen özel hücreye göre iþlemler

    // normal hücre fonksiyonu
    private void handleNormalCell(List<Cell> matchingCells)
    {
        // Eðer zincirdeki hücre sayýsý yeterince büyükse (örneðin 2'den fazla)
        if (matchingCells.Count > 1)
        {
            scoreManager.AddScore(matchingCells.Count);
            foreach (Cell cell in matchingCells)
            {
                // Hücreleri yok et (örneðin grid dizisinden sil ve ekrandan kaldýr)
                grid[cell.row, cell.column] = null;
                Destroy(cell.gameObject);
            }

            // Hücreleri aþaðý düþürme
            DropCells();
        }

        StartCoroutine(RefillGrid()); // Boþ kalan yerleri doldur
    }

    // patlama hücresi fonksiyonu
    private void HandleExplosiveCell(int row, int column)
    {
        // 1 x 1 basýldý 0 x 0 dan baþladý ve 2 x 2 ye kadar döngüye alýp listeye ekledi.
        // 0 x 0, 0 x 1, 0 x 2 column
        // 0 x 0, 1 x 0, 2 x 0
        List<Cell> affectedCells = new List<Cell>();

        // Çevredeki 3x3 bölgeyi tarýyoruz
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

    // joker hücresi fonksiyonu

    // renk deðiþtirme hücresi fonksiyonu
    private void HandleColorChangerCell(int row, int column)
    {
        Cell cell = grid[row, column];
        Color randomColor = possibleColors[Random.Range(0,possibleColors.Length)];
        cell.cellColor = randomColor;
        cell.GetComponent<SpriteRenderer>().color = randomColor;

    }

    // gelen özel hücreye göre iþlemler


    // Hücrenin etrafýndaki ayný renkli hücreleri tespit etme
    // OnCellClicked fonksiyonundan baþlangýç hücre bilgileri buraya gelir.
    private List<Cell> FindMatchingCells(int startRow, int startColumn, Color targetColor)
    {
        // ayný renkteki hücrelerin listesi
        List<Cell> matchingCells = new List<Cell>();
        // ziyaret edilen hücrelerin kümesi
        HashSet<Cell> visited = new HashSet<Cell>();

        // parametre olarak gelen baþlangýç hücre bilgileri ve liste - küme özyinelemeli fonksiyona gider.
        FloodFill(startRow, startColumn, targetColor, matchingCells, visited);

        return matchingCells;
    }

    // FindMatchingCells fonksiyonundan buraya parametreler gelicek
    private void FloodFill(int row, int column, Color targetColor, List<Cell> matchingCells, HashSet<Cell> visited)
    {
        // Grid sýnýrlarýnýn dýþýnda mýyýz?
        if (row < 0 || row >= rows || column < 0 || column >= columns)
            return;

        // Hücre var mý ve daha önce kontrol edildi mi?
        Cell cell = grid[row, column]; // iþlem yapýlan hücreyi ifade eder.
        if (cell == null || visited.Contains(cell)) // hücre null'sa ya da visited yani ziyaret edilen küme içinde
                                                    // varsa bu adýmý atlar ve devam eder.
            return;

        // Hücrenin rengi eþleþiyor mu?
        // eþleþmiyorsa bu adýmý atla ve devam et
        if (cell.cellColor != targetColor)
            return;

        // Hücreyi eþleþenler listesine ekle ve kontrol edilmiþ olarak iþaretle
        // hücre kontrolleri atlatýp buraya kadar geldiyse listeye ve kümeye ekle.
        matchingCells.Add(cell);
        visited.Add(cell);

        // Dört yönlü kontrol (üst, alt, sað, sol)
        FloodFill(row + 1, column, targetColor, matchingCells, visited);
        FloodFill(row - 1, column, targetColor, matchingCells, visited);
        FloodFill(row, column + 1, targetColor, matchingCells, visited);
        FloodFill(row, column - 1, targetColor, matchingCells, visited);

        // Çapraz yön kontrolü
        FloodFill(row + 1, column + 1, targetColor, matchingCells, visited);
        FloodFill(row + 1, column - 1, targetColor, matchingCells, visited);
        FloodFill(row - 1, column + 1, targetColor, matchingCells, visited);
        FloodFill(row - 1, column - 1, targetColor, matchingCells, visited);
    }

    private void DropCells()
    {
        for (int column = 0; column < columns; column++)
        {
            int emptyRow = -1; // Boþ bir satýr bulmak için
            for (int row = 0; row < rows; row++)
            {
                if (grid[row, column] == null)
                {
                    if (emptyRow == -1) emptyRow = row; // Ýlk boþ satýrý belirle
                }
                else if (emptyRow != -1) // Boþ satýr bulunduktan sonra dolu hücreyi kaydýr
                {
                    // Hücreyi aþaðý taþý
                    StartCoroutine(MoveCellDown(grid[row, column], emptyRow));
                    grid[emptyRow, column] = grid[row, column];
                    grid[row, column] = null;
                    grid[emptyRow, column].row = emptyRow; // Yeni pozisyonu güncelle
                    emptyRow++; // Bir sonraki boþ satýra geç
                }
            }
        }
    }

    // Hücreyi yavaþça düþürmek için Coroutine
    private IEnumerator MoveCellDown(Cell cell, int targetRow)
    {
        Vector2 startPosition = cell.transform.position; // Baþlangýç pozisyonu
        Vector2 endPosition = new Vector2(cell.column * cellSize.x, targetRow * cellSize.y) - new Vector2((columns - 1) * cellSize.x / 2, (rows - 1) * cellSize.y / 2); // Hedef pozisyon

        float duration = 0.5f; // Hücre düþüþ süresi (yarým saniye)
        float elapsedTime = 0f; // Geçen zaman

        while (elapsedTime < duration)
        {
            // Pozisyonu yavaþça güncelle
            cell.transform.position = Vector2.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime; // Zamaný artýr
            yield return null; // Bir frame bekle
        }

        cell.transform.position = endPosition; // Hedef pozisyona tam olarak yerleþtir
    }

    private IEnumerator RefillGrid()
    {
        yield return new WaitForSeconds(1f); // Bekleme süresi, oyun akýþýna göre düzenlenebilir

        // Yeni hücrelerin grid'e eklenmesi
        for (int column = 0; column < columns; column++)
        {
            for (int row = 0; row < rows; row++)
            {
                if (grid[row, column] == null) // Eðer grid hücresi boþsa
                {
                    // Pozisyon hesaplamasý: Grid'in ortasýna göre hizalý
                    Vector2 spawnPosition = GetSpawnPosition(row, column);

                    // Yeni hücre oluþturma ve konumlandýrma
                    GameObject newCellObj = CreateCell(spawnPosition, row, column);

                    // Yeni hücreyi grid'e ekle
                    grid[row, column] = newCellObj.GetComponent<Cell>();
                }
            }
        }
    }

    // Hücre oluþturma iþlemini modüler hale getirdik
    private GameObject CreateCell(Vector2 position, int row, int column)
    {
        GameObject newCellObj = Instantiate(cellPrefab, position, Quaternion.identity, transform);
        newCellObj.name = $"Cell_{row}_{column}";

        // Hücre için bileþen kontrolleri ve eklemeleri
        if (newCellObj.GetComponent<BoxCollider2D>() == null)
        {
            newCellObj.AddComponent<BoxCollider2D>();
        }

        Cell cellScript = newCellObj.AddComponent<Cell>();
        SpriteRenderer sr = newCellObj.GetComponent<SpriteRenderer>() ?? newCellObj.AddComponent<SpriteRenderer>();

        // Rastgele renk seçimi ve hücreye atanmasý
        Color randomColor = possibleColors[Random.Range(0, possibleColors.Length)];
        sr.color = new Color(randomColor.r, randomColor.g, randomColor.b, 1f);

        // Hücreyi baþlat
        cellScript.Init(row, column, sr.color, this);

        return newCellObj;
    }

    // Grid'e göre spawn pozisyonunu hesaplayan metod
    private Vector2 GetSpawnPosition(int row, int column)
    {
        return new Vector2(
            column * cellSize.x - (columns - 1) * cellSize.x / 2,
            row * cellSize.y - (rows - 1) * cellSize.y / 2
        );
    }
}
