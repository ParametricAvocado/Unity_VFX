namespace DevonaProject {
    [System.Serializable]
    public class ComboNodeTransition {
        public ComboNode targetNode;
        public float transitionBegin;
        public float transitionEnd;
        public ComboInput input;

        public ComboNodeTransition(ComboNode targetNode, float transitionBegin, float transitionEnd, ComboInput input) {
            this.targetNode = targetNode;
            this.transitionBegin = transitionBegin;
            this.transitionEnd = transitionEnd;
            this.input = input;
        }
    }
}