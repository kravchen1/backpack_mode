using UnityEngine;
using UnityEngine.UI;
using static TopDownCharacterController;

public class RotateCustomizationCharacter : MonoBehaviour
{
    #region Serialized Fields
    public int bodyIndex = 2;
    public Color bodyColor = Color.white;
    public Color headColor = Color.white;
    public int hairIndex = 0;
    public Color hairColor = Color.white;
    public int eyeIndex = 1;
    public Color eyeColor = Color.white;
    public Image head;
    public Image body;
    public Image hair;
    public Image eye;
    #endregion

    #region Private Variables
    private MovementDirection currentDirection = MovementDirection.Down;
    #endregion

    #region Public API Methods - Index Updates
    public void UpdateBodyIndex(int newBodyIndex)
    {
        bodyIndex = newBodyIndex;
        UpdateAppearanceSprites(currentDirection);
    }

    public void UpdateHairIndex(int newHairIndex)
    {
        hairIndex = newHairIndex;
        UpdateAppearanceSprites(currentDirection);
    }

    public void UpdateEyeIndex(int newEyeIndex)
    {
        eyeIndex = newEyeIndex;
        UpdateAppearanceSprites(currentDirection);
    }
    #endregion

    #region Public API Methods - Color Updates
    public void UpdateBodyColor(Color newColor)
    {
        bodyColor = newColor;
        UpdateAppearanceSprites(currentDirection);
    }

    public void UpdateHeadColor(Color newColor)
    {
        headColor = newColor;
        UpdateAppearanceSprites(currentDirection);
    }

    public void UpdateHairColor(Color newColor)
    {
        hairColor = newColor;
        UpdateAppearanceSprites(currentDirection);
    }

    public void UpdateEyeColor(Color newColor)
    {
        eyeColor = newColor;
        UpdateAppearanceSprites(currentDirection);
    }
    #endregion

    #region Public API Methods - Complex Operations
    public void UpdateAllAppearance(int newBodyIndex, int newHairIndex, int newEyeIndex)
    {
        bodyIndex = newBodyIndex;
        hairIndex = newHairIndex;
        eyeIndex = newEyeIndex;
        UpdateAppearanceSprites(currentDirection);
    }

    public void RefreshAppearance()
    {
        UpdateAppearanceSprites(currentDirection);
    }

    public void RotateLeft()
    {
        currentDirection = GetNextDirection(currentDirection, true);
        UpdateAppearanceSprites(currentDirection);
    }

    public void RotateRight()
    {
        currentDirection = GetNextDirection(currentDirection, false);
        UpdateAppearanceSprites(currentDirection);
    }
    #endregion

    #region Private Methods - Rotation Logic
    private MovementDirection GetNextDirection(MovementDirection current, bool rotateLeft)
    {
        if (rotateLeft)
        {
            switch (current)
            {
                case MovementDirection.Down: return MovementDirection.Left;
                case MovementDirection.Left: return MovementDirection.Up;
                case MovementDirection.Up: return MovementDirection.Right;
                case MovementDirection.Right: return MovementDirection.Down;
                default: return MovementDirection.Down;
            }
        }
        else
        {
            switch (current)
            {
                case MovementDirection.Down: return MovementDirection.Right;
                case MovementDirection.Right: return MovementDirection.Up;
                case MovementDirection.Up: return MovementDirection.Left;
                case MovementDirection.Left: return MovementDirection.Down;
                default: return MovementDirection.Down;
            }
        }
    }
    #endregion

    #region Private Methods - Appearance Updates
    private void UpdateAppearanceSprites(MovementDirection movementDir)
    {
        if (CharacterAppearanceManager.Instance == null) return;

        // Обновляем спрайты в зависимости от направления
        UpdateSpritesBasedOnDirection(movementDir);

        // Обновляем scale для отражения спрайтов
        UpdateSpritesScale(movementDir);

        // Применяем цвета
        ApplyColors();
    }

    private void UpdateSpritesBasedOnDirection(MovementDirection movementDir)
    {
        switch (movementDir)
        {
            case MovementDirection.Up:
                head.sprite = CharacterAppearanceManager.Instance.GetHeadUp();
                body.sprite = CharacterAppearanceManager.Instance.GetBodyUp(bodyIndex);
                hair.gameObject.SetActive(hairIndex != 0);
                hair.sprite = CharacterAppearanceManager.Instance.GetHairUp(hairIndex);
                eye.gameObject.SetActive(false);
                break;

            case MovementDirection.Down:
                head.sprite = CharacterAppearanceManager.Instance.GetHeadDown();
                body.sprite = CharacterAppearanceManager.Instance.GetBodyDown(bodyIndex);
                hair.gameObject.SetActive(hairIndex != 0);
                hair.sprite = CharacterAppearanceManager.Instance.GetHairDown(hairIndex);
                eye.gameObject.SetActive(true);
                eye.sprite = CharacterAppearanceManager.Instance.GetEyeDown(eyeIndex);
                break;

            case MovementDirection.Right:
            case MovementDirection.Left:
                head.sprite = CharacterAppearanceManager.Instance.GetHeadSide();
                body.sprite = CharacterAppearanceManager.Instance.GetBodySide(bodyIndex);
                hair.gameObject.SetActive(hairIndex != 0);
                hair.sprite = CharacterAppearanceManager.Instance.GetHairSide(hairIndex);
                eye.gameObject.SetActive(true);
                eye.sprite = CharacterAppearanceManager.Instance.GetEyeSide(eyeIndex);
                break;
        }
    }

    private void UpdateSpritesScale(MovementDirection movementDir)
    {
        float scaleX = movementDir == MovementDirection.Right ? -1f : 1f;

        Vector3 headScale = head.transform.localScale;
        Vector3 bodyScale = body.transform.localScale;
        Vector3 hairScale = hair.transform.localScale;
        Vector3 eyeScale = eye.transform.localScale;

        head.transform.localScale = new Vector3(scaleX, headScale.y, headScale.z);
        body.transform.localScale = new Vector3(scaleX, bodyScale.y, bodyScale.z);
        //hair.transform.localScale = new Vector3(scaleX, hairScale.y, hairScale.z);
        //eye.transform.localScale = new Vector3(scaleX, eyeScale.y, eyeScale.z);
    }

    private void ApplyColors()
    {
        body.color = bodyColor;
        head.color = headColor;
        hair.color = hairColor;
        eye.color = eyeColor;
    }
    #endregion
}