using UnityEngine;

namespace ValheimBossTrader
{
    /// <summary>
    /// Fenêtre bancaire style Haldor — bois sombre, forêt noire.
    /// IMGUI pur, aucune dépendance UnityEngine.UI.
    /// </summary>
    public class BankUI : MonoBehaviour
    {
        public static BankUI Instance { get; private set; }

        private bool   _visible;
        private string _amountStr  = "";
        private string _feedback   = "";
        private bool   _feedbackOk = true;

        private Rect _windowRect;
        private bool _stylesReady;

        // Styles
        private GUIStyle _windowStyle;
        private GUIStyle _titleStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _balanceStyle;
        private GUIStyle _fieldStyle;
        private GUIStyle _btnStyle;
        private GUIStyle _btnCloseStyle;
        private GUIStyle _separatorStyle;
        private GUIStyle _feedbackStyle;

        // Palettee Haldor / Forêt Noire
        private static readonly Color cDarkWood   = new Color(0.07f, 0.042f, 0.015f, 0.97f);
        private static readonly Color cMedWood    = new Color(0.16f, 0.100f, 0.038f, 1.00f);
        private static readonly Color cBtnNormal  = new Color(0.20f, 0.120f, 0.045f, 1.00f);
        private static readonly Color cBtnHover   = new Color(0.32f, 0.200f, 0.075f, 1.00f);
        private static readonly Color cBtnActive  = new Color(0.46f, 0.290f, 0.090f, 1.00f);
        private static readonly Color cAmber      = new Color(0.50f, 0.310f, 0.080f, 1.00f);
        private static readonly Color cGold       = new Color(0.88f, 0.680f, 0.180f, 1.00f);
        private static readonly Color cCream      = new Color(0.84f, 0.755f, 0.560f, 1.00f);
        private static readonly Color cDimAmber   = new Color(0.70f, 0.510f, 0.210f, 1.00f);
        private static readonly Color cInputBg    = new Color(0.04f, 0.024f, 0.008f, 1.00f);

        // ── Cycle de vie ──────────────────────────────────────────────────────

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _windowRect = new Rect(Screen.width - 350f, Screen.height / 2f - 165f, 310f, 330f);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        // ── API publique ──────────────────────────────────────────────────────

        public void Show()  { _feedback = ""; _amountStr = ""; _visible = true; }
        public void Hide()  => _visible = false;

        // ── IMGUI ─────────────────────────────────────────────────────────────

        private void OnGUI()
        {
            if (!_visible) return;
            EnsureStyles();
            _windowRect = GUI.Window(9421, _windowRect, DrawWindow, "", _windowStyle);
        }

        private void DrawWindow(int id)
        {
            // ── Barre de titre ─────────────────────────────────────────────
            GUI.DrawTexture(new Rect(0, 0, _windowRect.width, 38), MakeTex(1, 1, cMedWood));
            GUI.Label(new Rect(0, 5, _windowRect.width, 28), "⚜  COFFRE D'HALDOR  ⚜", _titleStyle);

            // ── Séparateur doré ────────────────────────────────────────────
            GUI.DrawTexture(new Rect(8, 38, _windowRect.width - 16, 2), MakeTex(1, 1, cAmber));

            // Espace sous le header
            GUILayout.Space(52);

            // ── Solde ──────────────────────────────────────────────────────
            GUILayout.BeginHorizontal();
            GUILayout.Space(12);
            GUILayout.Label($"✦  {BankManager.GetBalance():N0}  pièces", _balanceStyle,
                GUILayout.Width(_windowRect.width - 24));
            GUILayout.EndHorizontal();

            GUILayout.Space(4);
            // Séparateur fin sous le solde
            GUILayout.Box("", _separatorStyle,
                GUILayout.Height(1), GUILayout.Width(_windowRect.width - 24));

            GUILayout.Space(10);

            // ── Champ montant ──────────────────────────────────────────────
            GUILayout.BeginHorizontal();
            GUILayout.Space(12);
            GUILayout.Label("Montant :", _labelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(12);
            _amountStr = GUILayout.TextField(_amountStr, 9, _fieldStyle,
                GUILayout.Height(30), GUILayout.Width(_windowRect.width - 28));
            GUILayout.Space(4);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // ── Boutons Déposer / Retirer ──────────────────────────────────
            GUILayout.BeginHorizontal();
            GUILayout.Space(12);
            if (GUILayout.Button("Déposer", _btnStyle, GUILayout.Height(34), GUILayout.Width(133)))
                OnDeposit();
            GUILayout.Space(6);
            if (GUILayout.Button("Retirer", _btnStyle, GUILayout.Height(34), GUILayout.Width(133)))
                OnWithdraw();
            GUILayout.Space(4);
            GUILayout.EndHorizontal();

            GUILayout.Space(6);

            // ── Tout retirer ───────────────────────────────────────────────
            GUILayout.BeginHorizontal();
            GUILayout.Space(12);
            if (GUILayout.Button("Tout retirer", _btnStyle,
                GUILayout.Height(28), GUILayout.Width(_windowRect.width - 28)))
                OnWithdrawAll();
            GUILayout.Space(4);
            GUILayout.EndHorizontal();

            GUILayout.Space(6);

            // ── Feedback ───────────────────────────────────────────────────
            if (!string.IsNullOrEmpty(_feedback))
            {
                _feedbackStyle.normal.textColor = _feedbackOk
                    ? new Color(0.48f, 0.82f, 0.30f)
                    : new Color(0.88f, 0.32f, 0.22f);
                GUILayout.BeginHorizontal();
                GUILayout.Space(12);
                GUILayout.Label(_feedback, _feedbackStyle,
                    GUILayout.Width(_windowRect.width - 28));
                GUILayout.EndHorizontal();
                GUILayout.Space(2);
            }

            // ── Fermer ─────────────────────────────────────────────────────
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Fermer", _btnCloseStyle, GUILayout.Height(24), GUILayout.Width(80)))
                Hide();
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            GUILayout.Space(6);

            GUI.DragWindow(new Rect(0, 0, _windowRect.width, 38));
        }

        // ── Actions ───────────────────────────────────────────────────────────

        private void OnDeposit()
        {
            if (!int.TryParse(_amountStr, out int amount))
            { SetFeedback("Entrez un nombre valide.", false); return; }
            SetFeedback(null, BankManager.Deposit(amount, out string msg), msg);
        }

        private void OnWithdraw()
        {
            if (!int.TryParse(_amountStr, out int amount))
            { SetFeedback("Entrez un nombre valide.", false); return; }
            SetFeedback(null, BankManager.Withdraw(amount, out string msg), msg);
        }

        private void OnWithdrawAll()
        {
            SetFeedback(null, BankManager.WithdrawAll(out string msg), msg);
        }

        private void SetFeedback(string direct, bool ok, string msg = null)
        {
            _feedback   = direct ?? msg;
            _feedbackOk = ok;
            if (ok) _amountStr = "";
        }

        // ── Styles ────────────────────────────────────────────────────────────

        private void EnsureStyles()
        {
            if (_stylesReady) return;
            _stylesReady = true;

            // Fenêtre principale
            _windowStyle = new GUIStyle(GUI.skin.window)
            {
                padding = new RectOffset(0, 0, 0, 6),
                border  = new RectOffset(4, 4, 4, 4)
            };
            _windowStyle.normal.background   = MakeTex(1, 1, cDarkWood);
            _windowStyle.onNormal.background = MakeTex(1, 1, cDarkWood);

            // Titre centré doré
            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 15,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            _titleStyle.normal.textColor = cGold;

            // Labels sections
            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 12,
                fontStyle = FontStyle.Bold
            };
            _labelStyle.normal.textColor = cDimAmber;

            // Solde (grand, centré)
            _balanceStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            _balanceStyle.normal.textColor = cCream;

            // Champ de saisie
            _fieldStyle = new GUIStyle(GUI.skin.textField)
            {
                fontSize  = 14,
                alignment = TextAnchor.MiddleCenter
            };
            _fieldStyle.normal.background  = MakeTex(1, 1, cInputBg);
            _fieldStyle.focused.background = MakeTex(1, 1, new Color(0.06f, 0.038f, 0.014f, 1f));
            _fieldStyle.hover.background   = MakeTex(1, 1, new Color(0.05f, 0.030f, 0.012f, 1f));
            _fieldStyle.normal.textColor   = cCream;
            _fieldStyle.focused.textColor  = Color.white;

            // Boutons bois
            _btnStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize  = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            _btnStyle.normal.background  = MakeTex(1, 1, cBtnNormal);
            _btnStyle.hover.background   = MakeTex(1, 1, cBtnHover);
            _btnStyle.active.background  = MakeTex(1, 1, cBtnActive);
            _btnStyle.normal.textColor   = cCream;
            _btnStyle.hover.textColor    = Color.white;
            _btnStyle.active.textColor   = cGold;

            // Bouton Fermer (plus petit, discret)
            _btnCloseStyle = new GUIStyle(_btnStyle)
            {
                fontSize  = 11,
                fontStyle = FontStyle.Normal
            };
            _btnCloseStyle.normal.textColor = new Color(0.65f, 0.55f, 0.38f);

            // Ligne de séparation
            _separatorStyle = new GUIStyle(GUI.skin.box)
            {
                padding = RectOffset(0),
                margin  = new RectOffset(12, 12, 0, 0),
                border  = RectOffset(0)
            };
            _separatorStyle.normal.background = MakeTex(1, 1, cAmber);

            // Feedback
            _feedbackStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 12,
                wordWrap  = true,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleLeft
            };
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static RectOffset RectOffset(int v) => new RectOffset(v, v, v, v);

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
