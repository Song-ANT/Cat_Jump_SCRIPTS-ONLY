using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class FeatherController : MonoBehaviour
{
    [SerializeField] private GameObject Kasha;

    private GameObject _cat;
    private Vector2 _initPosition;

    private float followTime = 1f;

    private void Start()
    {
        _cat = GameObject.FindWithTag("Cat");
        _initPosition = transform.position;
        followTime = Data_Manager.Instance.Upgrade_Feather_Level * 0.03f + 1;

        DownMove();
    }

    private void DownMove()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2 - 100);
        Vector2 worldCenter = Camera.main.ScreenToWorldPoint(screenCenter);

        transform.DOMove(worldCenter, 1f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            StartCoroutine(FollowCat());
        });
    }

    private void UpMove()
    {
        Kasha.SetActive(true);
        transform.DOMove(transform.position + Vector3.up * 10, 1f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }



    private IEnumerator FollowCat()
    {
        Kasha.SetActive(false);

        float elapsedTime = 0f;

        while (elapsedTime < followTime)
        {
            elapsedTime += Time.deltaTime;

            if (_cat != null)
            {
                transform.position = _cat.transform.position;
            }

            yield return null;
        }

        UpMove();
    }

}
