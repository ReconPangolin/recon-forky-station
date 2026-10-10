using Content.Shared.Chat;
using Content.Shared.Chat.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared.EntityEffects.Effects;

/// <summary>
/// Makes this entity emote.
/// </summary>
/// <inheritdoc cref="EntityEffectSystem{T,TEffect}"/>
public sealed partial class EmoteEntityEffectSystem : EntityEffectSystem<MetaDataComponent, Emote>
{
    [Dependency] private SharedChatSystem _chat = default!;

    protected override void Effect(Entity<MetaDataComponent> entity, ref EntityEffectEvent<Emote> args)
    {
        if (args.Effect.ShowInChat)
            _chat.TryEmoteWithChat(entity, args.Effect.EmoteId, ChatTransmitRange.GhostRangeLimit, forceEmote: args.Effect.Force, ignoreActionBlocker:args.Effect.IgnoreActionBlocker); // Funky - Added IgnoreActionBlocker
        else
            _chat.TryEmoteWithChat(entity, args.Effect.EmoteId, ChatTransmitRange.HideChat, forceEmote: args.Effect.Force, ignoreActionBlocker:args.Effect.IgnoreActionBlocker); // Funky - Added IgnoreActionBlocker
    }
}

/// <inheritdoc cref="EntityEffect"/>
public sealed partial class Emote : EntityEffectBase<Emote>
{
    /// <summary>
    ///     The emote the entity will preform.
    /// </summary>
    [DataField("emote", required: true)]
    public ProtoId<EmotePrototype> EmoteId;

    /// <summary>
    ///     If the emote should be recorded in chat.
    /// </summary>
    [DataField]
    public bool ShowInChat = false;

    /// <summary>
    ///     If the forced emote will be listed in the guidebook.
    /// </summary>
    [DataField]
    public bool ShowInGuidebook;

    /// <summary>
    ///     If true, the entity will preform the emote even if they normally can't.
    /// </summary>
    [DataField]
    public bool Force;

    //Funky start
    /// <summary>
    ///     If true, the entity will perform the emote even if they are blocked from performing emote actions.
    /// </summary>
    [DataField]
    public bool IgnoreActionBlocker = false;
    //Funky end

    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        if (!ShowInGuidebook || !prototype.Resolve(EmoteId, out var emote))
            return null; // JUSTIFICATION: Emoting is mostly flavor, so same reason popup messages are not in here.

        return Loc.GetString("entity-effect-guidebook-emote", ("chance", Probability), ("emote", Loc.GetString(emote.Name)));
    }
}
