using System;
using UnityEngine;

namespace ValheimBossTrader
{
    /// <summary>
    /// Panel de filtrage par catégorie, affiché à droite du StoreGui.
    /// Même charte graphique Haldor / Forêt Noire que BankUI.
    /// </summary>
    public class CategoryFilterUI : MonoBehaviour
    {
        public static CategoryFilterUI Instance { get; private set; }

        private bool _visible;
        private Rect _windowRect;
        private bool _stylesReady;
        private bool _positionSet;

        // Styles
        private GUIStyle _windowStyle;
        private GUIStyle _titleStyle;
        private GUIStyle _btnStyle;
        private GUIStyle _btnActiveStyle;
        private GUIStyle _separatorStyle;

        // Palette (identique à BankUI)
        private static readonly Color cDarkWood  = new Color(0.07f, 0.042f, 0.015f, 0.97f);
        private static readonly Color cMedWood   = new Color(0.16f, 0.100f, 0.038f, 1.00f);
        private static readonly Color cBtnNormal = new Color(0.20f, 0.120f, 0.045f, 1.00f);
        private static readonly Color cBtnHover  = new Color(0.32f, 0.200f, 0.075f, 1.00f);
        private static readonly Color cAmber     = new Color(0.50f, 0.310f, 0.080f, 1.00f);
        private static readonly Color cGold      = new Color(0.88f, 0.680f, 0.180f, 1.00f);
        private static readonly Color cCream     = new Color(0.84f, 0.755f, 0.560f, 1.00f);
        private static readonly Color cActiveText = new Color(0.10f, 0.06f, 0.02f, 1.00f);

        // ── Cycle de vie ──────────────────────────────────────────────────────

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _windowRect = new Rect(420, 130, 142, 0); // hauteur calculée dans EnsureStyles
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        // ── API publique ──────────────────────────────────────────────────────

        public void Show()
        {
            _visible     = true;
            _positionSet = false;   // recalculer la position à chaque ouverture
        }

        public void Hide()
        {
            _visible = false;
            CategoryFilter.Reset();   // restaure la liste complète
        }

        // ── IMGUI ─────────────────────────────────────────────────────────────

        private void OnGUI()
        {
            if (!_visible) return;
            EnsureStyles();
            TrySetPosition();
            _windowRect = GUI.Window(9424, _windowRect, DrawWindow, "", _windowStyle);
        }

        private void TrySetPosition()
        {
            if (_positionSet) return;
            if (StoreGui.instance?.m_rootPanel == null) return;

            var rt = StoreGui.instance.m_rootPanel.GetComponent<RectTransform>();
            if (rt == null) return;

            var corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            // corners[2] = top-right en screen space (y=0 en bas)
            _windowRect.x = corners[2].x + 6f;
            _windowRect.y = Screen.height - corners[2].y;
            _positionSet  = true;
        }

        private void DrawWindow(int id)
        {
            // ── Header ─────────────────────────────────────────────────────
            GUI.DrawTexture(new Rect(0, 0, _windowRect.width, 32), MakeTex(1, 1, cMedWood));
            GUI.Label(new Rect(0, 5, _windowRect.width, 22), "CATÉGORIES", _titleStyle);
            GUI.DrawTexture(new Rect(6, 32, _windowRect.width - 12, 2), MakeTex(1, 1, cAmber));

            GUILayout.Space(40);

            // ── Tout ───────────────────────────────────────────────────────
            DrawBtn("Tout", null);

            GUILayout.Space(3);
            GUILayout.Box("", _separatorStyle, GUILayout.Height(1), GUILayout.ExpandWidth(true));
            GUILayout.Space(3);

            // ── Catégories ─────────────────────────────────────────────────
            foreach (Category cat in Enum.GetValues(typeof(Category)))
            {
                DrawBtn(GetLabel(cat), cat);
                GUILayout.Space(2);
            }

            GUILayout.Space(6);

            GUI.DragWindow(new Rect(0, 0, _windowRect.width, 32));
        }

        private void DrawBtn(string label, Category? cat)
        {
            bool active = CategoryFilter.Active == cat;
            GUILayout.BeginHorizontal();
            GUILayout.Space(6);
            if (GUILayout.Button(label, active ? _btnActiveStyle : _btnStyle,
                    GUILayout.Height(28), GUILayout.Width(_windowRect.width - 14)))
            {
                // Clic sur la catégorie déjà active → retour à "Tout"
                CategoryFilter.SetCategory(active && cat != null ? null : cat);
            }
            GUILayout.Space(2);
            GUILayout.EndHorizontal();
            GUILayout.Space(1);
        }

        private static string GetLabel(Category cat) => cat switch
        {
            Category.Materials   => "Matériaux",
            Category.Food        => "Nourriture",
            Category.Weapons     => "Armes",
            Category.Armor       => "Armures",
            Category.Ammo        => "Munitions",
            Category.Consumables => "Consommables",
            Category.Misc        => "Divers",
            _                    => cat.ToString()
        };

        // ── Styles ────────────────────────────────────────────────────────────

        private void EnsureStyles()
        {
            if (_stylesReady) return;
            _stylesReady = true;

            // Calcul dynamique de la hauteur
            // btnHeight = bouton (28px) + GUILayout.Space(2) + marge interne IMGUI (~6px)
            int nbCats    = Enum.GetValues(typeof(Category)).Length;
            int btnHeight = 36;
            _windowRect.height = 32          // header + séparateur
                               + 44          // GUILayout.Space(40) + bouton Tout + espaces
                               + 16          // séparateur + GUILayout.Space(3)*2
                               + nbCats * btnHeight
                               + 20;         // padding bas IMGUI

            _windowStyle = new GUIStyle(GUI.skin.window)
            {
                padding = new RectOffset(0, 0, 0, 4)
            };
            _windowStyle.normal.background   = MakeTex(1, 1, cDarkWood);
            _windowStyle.onNormal.background = MakeTex(1, 1, cDarkWood);

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            _titleStyle.normal.textColor = cGold;

            _btnStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize  = 12,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleCenter
            };
            _btnStyle.normal.background = MakeTex(1, 1, cBtnNormal);
            _btnStyle.hover.background  = MakeTex(1, 1, cBtnHover);
            _btnStyle.active.background = MakeTex(1, 1, cAmber);
            _btnStyle.normal.textColor  = cCream;
            _btnStyle.hover.textColor   = Color.white;
            _btnStyle.active.textColor  = cGold;

            // Bouton actif : fond ambre, texte sombre
            _btnActiveStyle = new GUIStyle(_btnStyle)
            {
                fontStyle = FontStyle.Bold
            };
            _btnActiveStyle.normal.background = MakeTex(1, 1, cAmber);
            _btnActiveStyle.hover.background  = MakeTex(1, 1, new Color(0.60f, 0.38f, 0.10f, 1f));
            _btnActiveStyle.normal.textColor  = cActiveText;
            _btnActiveStyle.hover.textColor   = cActiveText;

            _separatorStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(0, 0, 0, 0),
                margin  = new RectOffset(6, 6, 0, 0),
                border  = new RectOffset(0, 0, 0, 0)
            };
            _separatorStyle.normal.background = MakeTex(1, 1, new Color(0.35f, 0.22f, 0.06f, 1f));
        }

        private static Texture2D MakeTex(int w, int h, Color col)
        {
            var tex = new Texture2D(w, h);
            var pix = new Color[w * h];
            for (int i = 0; i < pix.Length; i++) pix[i] = col;
            tex.SetPixels(pix);
            tex.Apply();
            return tex;
        }
    }
}
