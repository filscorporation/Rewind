namespace Source
{
    public class NPC : Unit
    {
        protected override float MaxHealth => 1;

        private bool isRescued = false;
        
        protected override void Die(bool blood = false)
        {
            base.Die(blood);
            
            RemoveSelf();
        }

        private void RemoveSelf()
        {
            base.Die();
            
            gameObject.SetActive(false);
        }

        public void Rescue()
        {
            GameManager.Instance.CurrentLevel.NPCRescued();
            isRescued = true;

            RemoveSelf();
        }

        public override RewindData GetData()
        {
            RewindData data = base.GetData();

            data.Data["isrescued"] = isRescued;
            
            return data;
        }

        public override void ApplyData(RewindData data)
        {
            base.ApplyData(data);

            bool oldIsRescued = isRescued;
            isRescued = (bool)data.Data["isrescued"];
            if (oldIsRescued && !isRescued)
            {
                gameObject.SetActive(true);
                GameManager.Instance.CurrentLevel.NPCUnrescued();
            }
        }
    }
}