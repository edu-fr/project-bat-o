using TMPro;
using UnityEngine;

namespace Resources.Project.Runtime.Scripts.UI
{
   public class LootPopup : MonoBehaviour
   {
      public TextMeshPro TextMeshPro;
      private static int SortingOrder;
      
      public TMP_FontAsset Font;
      public int FontSize; 
      public float MoveSpeed;
      public float DisappearTimerMax;  
      public float DisappearSpeed;
      public float IncreaseScaleAmount = 1f;
      // public float DecreaseScaleAmount = 1f;
      public Color Color;
      private Color TextColor;
      
      private Vector3 MoveVector; 
      private float DisappearTimer;
      
      // Create a damage popup
      public static LootPopup Create(Vector3 position, int experienceAmount, Transform prefabLootPopup)
      {
         Transform lootPopupTransform = Instantiate(prefabLootPopup, position, Quaternion.identity);
         LootPopup lootPopup = lootPopupTransform.GetComponent<LootPopup>();
         lootPopup.Setup(experienceAmount);
         return lootPopup;
      }
      
      public static LootPopup Create(Vector3 position, string itemName, Transform prefabLootPopup)
      {
         Transform lootPopupTransform = Instantiate(prefabLootPopup, position, Quaternion.identity);
         LootPopup lootPopup = lootPopupTransform.GetComponent<LootPopup>();
         lootPopup.Setup(itemName);
         return lootPopup;
      }
   
      private void Awake()
      {
         if (Font != null)
         {
            TextMeshPro.font = Font;
         }
      }

      public void Setup(int experienceAmount)
      {
         TextMeshPro.SetText("+" + experienceAmount + " EXP");
         TextMeshPro.fontSize = FontSize;
         TextColor = Color;
         TextMeshPro.color = TextColor;
         DisappearTimer = DisappearTimerMax;
         MoveVector = Vector3.up * MoveSpeed;
         SortingOrder++;
         TextMeshPro.sortingOrder = SortingOrder;
      }
      
      public void Setup(string itemName)
      {
         TextMeshPro.SetText("+1 " + itemName);
         TextMeshPro.fontSize = FontSize;
         TextColor = Color;
         TextMeshPro.color = TextColor;
         DisappearTimer = DisappearTimerMax;
         MoveVector = Vector3.up * MoveSpeed;
         SortingOrder++;
         TextMeshPro.sortingOrder = SortingOrder;
      }

      private void Update()
      {
         transform.position += MoveVector * Time.deltaTime;
         MoveVector -= MoveVector * (8f * Time.deltaTime);
         DisappearTimer -= Time.deltaTime;

         if (DisappearTimer > DisappearTimerMax * 0.5f)
         {
            // First half
            transform.localScale += Vector3.one * (IncreaseScaleAmount * Time.deltaTime);
         }

         if (DisappearTimer < 0)
         {
            // Start to disappear
            TextColor.a -= DisappearSpeed * Time.deltaTime;
            // TextMeshPro.color = TextColor;
            if (TextColor.a < 0)
            {
               Destroy(gameObject);
            }
         }
      }
   }
}
