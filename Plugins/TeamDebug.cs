namespace Oxide.Plugins
{
    [Info("Team Debug", "Ryz0r", "1.0.0")]
    [Description("Team Debug")]
    public class TeamDebug : RustPlugin
    {
        private object OnTeamCreate(BasePlayer player)
        {
            Puts($"{player} has created a new team");
            return null;
        }
        
        private object OnTeamUpdate(ulong currentTeam, ulong newTeam, BasePlayer player)
        {
            Puts($"{player} has changed their team from {currentTeam} to {newTeam}");
            return null;
        }
        
        private object OnTeamInvite(BasePlayer inviter, BasePlayer target)
        {
            Puts($"{inviter.displayName} invited {target.displayName} to his team");
            return null;
        }
        
        private object OnTeamAcceptInvite(RelationshipManager.PlayerTeam team, BasePlayer player)
        {
            Puts($"{player} was invited to a team with {team.members.Count} {team.teamID} {team.teamLeader}");
            return null;
        }
    }
}