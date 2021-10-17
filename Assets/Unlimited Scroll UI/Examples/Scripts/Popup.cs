using UnityEngine;
using UnityEngine.UI;

namespace UnlimitedScrollUI.Example {
    public class Popup : MonoBehaviour {
        public Text text;
        public Button btn;

        private void Start() {
            btn.onClick.AddListener(() => Destroy(gameObject));
        }
    }
}
