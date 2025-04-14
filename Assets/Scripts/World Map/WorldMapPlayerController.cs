using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Tilemaps;

public class WorldMapPlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float _mapMoveSpeed;

    [Header("References")]
    [SerializeField] GameObject _movePoint;
    [SerializeField] Tilemap _tileMap;

    private Camera _mainCamera;

    [Header("Data")]
    private bool _canMove;
    private bool _isMoving;

    private void Start()
    {
        _mainCamera = Camera.main;
        _movePoint.SetActive(false);

        _mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    }

    private void Update()
    {
        if (_isMoving)
        {
            return;
        }

        // Convert click position to world position on the tilemap plane
        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = _movePoint.transform.position.z; // Set Z to player's Z for tilemap check

        Vector3Int clickedCell = new();

        if (_tileMap.HasTile(_tileMap.WorldToCell(mouseWorldPos)))
        {
            clickedCell = _tileMap.WorldToCell(mouseWorldPos);
            Vector3Int playerCell = _tileMap.WorldToCell(transform.position);


            if (Mathf.Abs(clickedCell.x - playerCell.x) == 1 && clickedCell.y == playerCell.y ||
                Mathf.Abs(clickedCell.y - playerCell.y) == 1 && clickedCell.x == playerCell.x)
            {
                _movePoint.SetActive(true);
                _movePoint.transform.position = _tileMap.GetCellCenterWorld(clickedCell);
                _canMove = true;
            }
            else
            {
                _movePoint.SetActive(false);
                _canMove = false;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (_canMove)
            {
                _canMove = false;
                _movePoint.SetActive(false);

                // Move player to the clicked tile position
                StartCoroutine(MoveToNewPos(_tileMap.GetCellCenterWorld(clickedCell)));
                _movePoint.transform.position = _tileMap.GetCellCenterWorld(clickedCell);

                _mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<WorldMapIconController>() != null)
        {
            WorldMapIconController worldMapIconController = collision.GetComponent<WorldMapIconController>();
            //MessageBox.instance.Create("You are now entering " + worldMapIconController.GetIconLabel(), false);

            SceneLoader.instance.LoadPlayerScene(worldMapIconController.GetSceneInt(), "default", Vector3.zero, Vector3.zero, false, true);
        }
    }

    IEnumerator MoveToNewPos(Vector3 targetPosition)
    {
        _isMoving = true;

        float elapsedTime = 0;
        Vector3 currentPosition = transform.position;

        while (elapsedTime < _mapMoveSpeed)
        {
            transform.position = Vector3.Lerp(currentPosition, targetPosition, (elapsedTime / _mapMoveSpeed));
            elapsedTime += Time.deltaTime;

            // Yield here
            yield return null;
        }

        // Make sure we got there
        transform.position = targetPosition;
        _isMoving = false;
        yield return null;
    }
}
