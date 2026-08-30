using UnityEngine;
using animator; // Подключаем неймспейс / Include custom namespace

public class TestMove : MonoBehaviour
{
    private Anim anim;
    private string currentAnimState = "";

    void Start() => anim = GetComponent<Anim>();

    void Update()
    {
        float input = Input.GetAxisRaw("Horizontal");

        // Движение по горизонтали / Horizontal movement
        transform.Translate(Vector3.right * input * 5f * Time.deltaTime);

        // Разворот персонажа (инверсия) / Character flip (inversion)
        if (input > 0)
            transform.localScale = new Vector3(1f, 1f, 1f);  // Смотрит вправо / Facing right
        else if (input < 0)
            transform.localScale = new Vector3(-1f, 1f, 1f); // Инверсия влево / Inverted left

        if (input != 0)
        {
            if (currentAnimState != "pr")
            {
                currentAnimState = "pr";
                anim.SwitchAnimation("pr");
                anim.Play(); // Запускаем / Start playback
            }
        }
        else
        {
            if (currentAnimState != "Idle")
            {
                currentAnimState = "Idle";
                anim.SwitchAnimation("Idle");
                anim.Play(); // Запускаем / Start playback
            }
        }
    }
}
