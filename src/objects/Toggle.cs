namespace UILib {
    public class Toggle : Button {
        public bool value { get; private set; } = false;

        public Toggle() : base(Resources.checkMark) {
            AddListener(() => {
                SetValue(!value);
            });
            SetValue(false);
        }

        public void SetValue(bool value) {
            if (value == true) {
                image.Show();
            }
            else {
                image.Hide();
            }

            this.value = value;
        }
    }
}
