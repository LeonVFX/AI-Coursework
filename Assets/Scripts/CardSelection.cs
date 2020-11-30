using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelection : MonoBehaviour
{
    private RaycastHit hit;
    private Ray ray;
    private Rigidbody rbSelected;
    private Rigidbody rbDeselected;
    private float moveSpeed = 5.0f;
    private Vector3 prevPos = Vector3.zero;
    private Vector3 newPos = Vector3.zero;
    private bool moveCard = false;

    public GameObject selectedCard;

    private void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.transform.GetComponent<Rigidbody>() != rbSelected)
                {
                    if (hit.transform.tag != "AICard")
                    {
                        rbDeselected = rbSelected;
                        prevPos = newPos;
                        prevPos.y -= 0.05f;
                        rbSelected = hit.transform.GetComponent<Rigidbody>();
                        newPos = new Vector3(rbSelected.position.x, rbSelected.position.y + 0.05f, rbSelected.position.z);
                        selectedCard = rbSelected.gameObject;
                        moveCard = true;
                    }
                }
            }
        }

        if (moveCard)
            MoveSelectedCard();
    }

    private void MoveSelectedCard()
    {
        
        rbSelected.MovePosition(Vector3.MoveTowards(rbSelected.position, newPos, moveSpeed * Time.deltaTime));
        if (rbDeselected != null)
            rbDeselected.MovePosition(Vector3.MoveTowards(rbDeselected.position, prevPos, moveSpeed * Time.deltaTime));

        if (rbSelected.position.y >= rbSelected.position.y + 0.05f)
            moveCard = false;
    }

    public void UseCard()
    {
        selectedCard.GetComponent<Card>().UseCard();
    }
}
