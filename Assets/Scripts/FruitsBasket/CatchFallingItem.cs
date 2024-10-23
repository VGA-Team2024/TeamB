using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchFallingItem : MonoBehaviour
{
    private System.Action onGetAction;
    private System.Action onDeleteAction;

    public void Setting(System.Action getaction, System.Action deleteaction)
    {
        onGetAction = getaction;
        onDeleteAction = deleteaction;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        //ÉvÉåÉCÉÑÅ[Ç∆ÇÃîªíË
        if (col.transform.name == "Player")
        {
            Debug.Log("PlayerHit");
            if (onGetAction != null)
                onGetAction();

            Delete();
        }

        //  è∞Ç∆ÇÃîªíË
        if (col.transform.name == "Floor")
        {
            Debug.Log("FloorHit");
            if(onDeleteAction != null)
                onDeleteAction();

            Delete();
        }
    }

    private void Delete()
    {
        GameObject.Destroy(this.gameObject);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
