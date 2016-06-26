namespace HouraiTeahouse.SmashBrew {
    //TODO: Document
    public interface IHitboxController : IRegistrar<Hitbox> {
        Hitbox GetHitbox(int id);
    }
}
