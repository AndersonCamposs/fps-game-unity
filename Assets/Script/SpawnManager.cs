using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float initialSpawnTime = 4.0f;
    public float minSpawnTime = 0.8f;
    void Start()
    {
        // Inicia a rotina que fica gerando inimigos continuamente
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float tempoEsperaAtual = CalculateWaitTime();

            yield return new WaitForSeconds(tempoEsperaAtual);

            SpawnEnemy();
        }
    }

    private float CalculateWaitTime()
    {
        int qtdInimigosDerrotados = UIManager.Instance.quantidadeInimigosDerrotados;

        // Diminui 0.2 segundos a cada inimigo morto (ajuste esse valor como preferir)
        float tempoReduzido = initialSpawnTime - (qtdInimigosDerrotados * 0.2f);

        // Garante que o jogo não fique impossível (não deixa o tempo ser menor que o mínimo)
        return Mathf.Max(tempoReduzido, minSpawnTime);
    }
    
    private void SpawnEnemy()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("Nenhum ponto de spawn foi definido no SpawnManager!");
            return;
        }

        int indexAleatorio = Random.Range(0, spawnPoints.Length);
        Transform pontoEscolhido = spawnPoints[indexAleatorio];

        Instantiate(enemyPrefab, pontoEscolhido.position, pontoEscolhido.rotation);
    }
}
