using TMPro;
using UnityEngine;

namespace Resources.Project.Runtime.Scripts.UI
{
   public class DamagePopup : MonoBehaviour
   {
      private TextMeshPro TextMeshPro;
      private static int SortingOrder;
      
      public TMP_FontAsset Font;
      [SerializeField] private float FontSize;
      [SerializeField] private float DotFontSize;
      [SerializeField] private float CriticalHitFontSize; 
      
      public float MoveSpeed;
      public float DisappearTimerMax;  
      public float DisappearSpeed;
      public float IncreaseScaleAmount = 1f;
      public float DecreaseScaleAmount = 1f;
      
      [SerializeField] private Color NormalDamageColor;
      [SerializeField] private Color CriticalDamageColor;
      [SerializeField] private Color DotDamageColor = Color.grey;
      
      private Vector3 MoveVector; 
      private float DisappearTimer;
      private Color TextColor;
      
      // Create a damage popup
      public static DamagePopup Create(Vector3 position, int damageAmount, Vector3 hitDirection, Transform prefabDamagePopup, bool isCriticalHit, bool isDot, Color? customColor)
      {
         var damagePopupTransform = Instantiate(prefabDamagePopup, position, Quaternion.identity);
         var damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
         damagePopup.Setup(damageAmount, hitDirection, isCriticalHit, isDot, customColor);
         return damagePopup;
      }
   
      private void Awake()
      {
         TextMeshPro = transform.GetComponent<TextMeshPro>();
         // TextMeshPro.font = Font;
      }

      private void Setup(int damageAmount, Vector3 hitDirection, bool isCriticalHit, bool isDot, Color? customColor)
      {
         TextMeshPro.SetText(damageAmount.ToString());

         if (isDot)              // Dot
         {
            TextMeshPro.fontSize = DotFontSize;
            TextColor = customColor?? NormalDamageColor;
         }
         else
         {
            if (isCriticalHit)   // Critical hit
            {
               TextMeshPro.fontSize = CriticalHitFontSize;
               TextColor = customColor?? CriticalDamageColor;
            }
            else                 // Normal hit   
            {
               TextMeshPro.fontSize = FontSize;
               TextColor = customColor?? NormalDamageColor;
            }
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
