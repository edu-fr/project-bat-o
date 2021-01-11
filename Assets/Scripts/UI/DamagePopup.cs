using Game;
using TMPro;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace UI
{
   public class DamagePopup : MonoBehaviour
   {
      private TextMeshPro TextMeshPro;
      private static int SortingOrder;
      
      public TMP_FontAsset Font;
      public int FontSize; 
      public int CriticalHitFontSize; 
      public float MoveSpeed;
      public float DisappearTimerMax;  
      public float DisappearSpeed;
      public float IncreaseScaleAmount = 1f;
      public float DecreaseScaleAmount = 1f;
      public Color NormalDamageColor;
      public Color CriticalDamageColor;
      
      private Vector3 MoveVector; 
      private float DisappearTimer;
      private Color TextColor;
      
      // Create a damage popup
      public static DamagePopup Create(Vector3 position, int damageAmount, bool isCriticalHit, Vector3 hitDirection, Transform prefabDamagePoput)
      {
         Transform damagePopupTransform = Instantiate(prefabDamagePoput, position, Quaternion.identity);
         DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
         damagePopup.Setup(damageAmount, isCriticalHit, hitDirection);
         return damagePopup;
      }
   
      private void Awake()
      {
         TextMeshPro = transform.GetComponent<TextMeshPro>();
         // TextMeshPro.font = Font;
      }

      public void Setup(int damageAmount, bool isCriticalHit, Vector3 hitDirection)
      {
         TextMeshPro.SetText(damageAmount.ToString());
         if (!isCriticalHit)
         {
            // Normal hit   
            TextMeshPro.fontSize = FontSize;
            TextColor = NormalDamageColor;
         }
         else
         {
            // Critical hit
            TextMeshPro.fontSize = CriticalHitFontSize;
            TextColor = CriticalDamageColor;
         }
         TextMeshPro.color = TextColor;
         DisappearTimer = DisappearTimerMax;
         MoveVector = hitDirection * MoveSpeed;
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
         else
         {
            // Second half
            transform.localScale -= Vector3.one * (DecreaseScaleAmount * Time.deltaTime);
         }
         
         if (DisappearTimer < 0)
         {
            // Start to disappear
            TextColor.a -= DisappearSpeed * Time.deltaTime;
            TextMeshPro.color = TextColor;
            if (TextColor.a < 0)
            {
               Destroy(gameObject);
            }
         }
      }
   }
}
